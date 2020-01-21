using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;

namespace PlClr
{
    internal static class CSharpCompiler
    {
        public static FunctionCallDelegate Compile(FunctionCompileInfo func)
        {
            var className = $"PlClrClass_{func.FunctionOid}";
            var safeMethodName = GetSafeFunctionName(func.FunctionName);
            var returnType = ServerTypes.GetTypeForOid(func.ReturnValueType);
            var returnTypeName = returnType == typeof(void) ? "void" : returnType.FullName;
            var builder = new StringBuilder()
                .Append($"using {nameof(PlClr)};\n")
                .Append("using System;\n\n")
                .Append("public static class ")
                .Append(className)
                .Append("\n{\n")
                .Append("\tpublic static IntPtr Execute_")
                .Append(safeMethodName)
                .Append("(NullableDatum[] values)\n")
                .Append("\t{\n");

            if (func.ArgumentOids.Any())
            {
                builder.Append("\t\t");
                if (func.IsStrict)
                {
                    builder.AppendJoin($"\n\t\t",
                        func.ArgumentOids.Select((oid, index) =>
                        $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = {nameof(ServerFunctions)}.{ServerTypes.GetValueAccessMethodForOid(oid)}(values[{index}].Value);"
                        ));
                }
                else
                {
                    builder.AppendJoin($"\n\t\t",
                            func.ArgumentOids.Select((oid, index) =>
                                $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = values[{index}].IsNull ? ({ServerTypes.GetTypeForOid(oid).FullName}?)null : {nameof(ServerFunctions)}.{ServerTypes.GetValueAccessMethodForOid(oid)}(values[{index}].Value);"));
                }
                builder.Append("\n\n");
            }
            builder.Append(returnType == typeof(void) ? $"\t\t{safeMethodName}(" : $"\t\tvar result = {safeMethodName}(")
                .AppendJoin(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            func.ArgumentNames?[index] ?? $"arg{index + 1}"))
                .Append(");\n");
            if (returnType == typeof(void))
                builder.Append("\t\treturn IntPtr.Zero;\n");
            else
            {
                if (!func.IsStrict)
                    builder.Append("\t\tif (result == null)\n")
                        .Append("\t\t{\n")
                        .Append("\t\t\treturn IntPtr.Zero;\n")
                        .Append("\t\t}\n\n");
                builder.Append(
                        "\t\treturn ")
                    .Append(nameof(ServerFunctions))
                    .Append('.')
                    .Append(nameof(ServerFunctions.GetDatum))
                    .Append('(');
                if (!func.IsStrict)
                    builder.Append("(")
                    .Append(returnType.FullName)
                    .Append(")");
                builder.Append("result);\n");
            }
            builder.Append("\t}\n\n")
                .Append("\tprivate static ")
                .Append(returnTypeName)
                .Append(returnType == typeof(void) || func.IsStrict ? " " : "? ")
                .Append(safeMethodName)
                .Append("(")
                .AppendJoin(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            $"{ServerTypes.GetTypeForOid(oid).FullName}{(func.IsStrict ? string.Empty : "?")} {func.ArgumentNames?[index] ?? $"arg{index + 1}"}"))
                .Append(")\n")
                .Append("\t{\n");
            if (func.FunctionBody.Length > 0)
            {
                builder.Append(func.FunctionBody);

                if (builder[^1] != '\n')
                    builder.Append('\n');
            }
            builder.Append("\t}\n")
                .Append("}\n");

            var generatedCode = builder.ToString();
            ServerLog.ELog(SeverityLevel.Debug1, $"PL/CLR generated code:\n{generatedCode}");
            var tree = CSharpSyntaxTree.ParseText(generatedCode);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var system = MetadataReference.CreateFromFile(typeof(Console).Assembly.Location);
            var systemRuntime = MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"));
            var plclr = MetadataReference.CreateFromFile(typeof(PlClrMain).Assembly.Location);
            var compilation = CSharpCompilation.Create($"PlClrAssembly_{func.FunctionOid}",
                syntaxTrees: new[] { tree }, references: new[] { mscorlib, system, systemRuntime, plclr },
                options: options);

            var ms = new MemoryStream();
            var res = compilation.Emit(ms);
            ms.Seek(0L, SeekOrigin.Begin);

            // Hack: We just throw out the raw diagnostics for now
            // ToDo: Think about it and consult users/peers what they'd expect/prefer
            if (res.Success)
            {
                static SeverityLevel GetSeverityLevel(DiagnosticSeverity severity)
                    => severity switch
                    {
                        DiagnosticSeverity.Hidden => SeverityLevel.Debug1,
                        DiagnosticSeverity.Info => SeverityLevel.Info,
                        DiagnosticSeverity.Warning => SeverityLevel.Warning,
                        DiagnosticSeverity.Error => SeverityLevel.Error,
                        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
                    };

                foreach (var resultDiagnostic in res.Diagnostics
                    .Where(d => !d.IsSuppressed && d.Severity != DiagnosticSeverity.Hidden)
                    .OrderBy(d => d.Location.IsInSource ? d.Location.SourceSpan.Start : int.MaxValue))
                    ServerLog.EReport(GetSeverityLevel(resultDiagnostic.Severity), errorMessageInternal: resultDiagnostic.ToString());

                var assembly = AssemblyLoadContext.GetLoadContext(typeof(CSharpCompiler).Assembly)!.LoadFromStream(ms);
                return (FunctionCallDelegate)assembly.GetType(className)!.GetMethod($"Execute_{safeMethodName}")!.CreateDelegate(typeof(FunctionCallDelegate));
            }

            static string GetDiagnostics(EmitResult result, DiagnosticSeverity level)
                => string.Join(Environment.NewLine,
                    result.Diagnostics
                        .Where(d => !d.IsSuppressed && d.Severity == level)
                        .OrderBy(d => d.Location.IsInSource ? d.Location.SourceSpan.Start : int.MaxValue)
                        .Select(d => d.ToString()));

            var infos = GetDiagnostics(res, DiagnosticSeverity.Info);
            if (infos.Length > 0)
            {
                Debug.WriteLine(infos);
                ServerLog.ELog(SeverityLevel.Info, infos);
            }

            var warnings = GetDiagnostics(res, DiagnosticSeverity.Warning);
            if (warnings.Length > 0)
            {
                Debug.WriteLine(warnings);
                ServerLog.ELog(SeverityLevel.Warning, warnings);
            }

            var errors = GetDiagnostics(res, DiagnosticSeverity.Error);
            if (errors.Length > 0)
            {
                Debug.WriteLine(errors);
                ServerLog.ELog(SeverityLevel.Error, errors);
            }

            // unreachable as Elog >= Error will tear down the process.
            throw new Exception("Unreachable");
        }

        private static string GetSafeFunctionName(string functionName)
            => new string(functionName.Take(1).Select(c => char.IsLetter(c) ? c : '_').Concat(functionName.Skip(1).Select(c => char.IsLetterOrDigit(c) ? c : '_')).ToArray());
    }
}
