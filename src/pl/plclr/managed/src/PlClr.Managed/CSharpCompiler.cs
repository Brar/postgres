using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlClr
{
    internal static class CSharpCompiler
    {
        public static Delegate? Compile(FunctionCompileInfo func)
        {
            var className = $"PlClrClass_{func.FunctionOid}";
            var safeMethodName = GetSafeFunctionName(func.FunctionName);
            var returnType = ServerTypes.GetTypeForOid(func.ReturnValueType);
            var returnTypeName = returnType == typeof(void) ? "void" : returnType.FullName;
            var builder = new StringBuilder()
                .AppendLine("using PlClr;")
                .AppendLine("using System;")
                .AppendLine()
                .Append("public static class ")
                .AppendLine(className)
                .AppendLine("{")
                .Append("\tpublic static IntPtr Execute_")
                .Append(safeMethodName)
                .Append("(ReadOnlySpan<NullableDatum> values)")
                .AppendLine("\t{")
                .Append("\t\t")
                .AppendLine(
                    string.Join($"{Environment.NewLine}\t\t",
                        func.ArgumentOids.Select((oid, index) =>
                            $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = values[{index}].IsNull ? ({ServerTypes.GetTypeForOid(oid).FullName}?)null : ServerFunction.{ServerTypes.GetValueAccessMethodForOid(oid)}(values[{index}].Value);")))
                .AppendLine()
                .Append(returnType == typeof(void) ? $"\t\t{safeMethodName}(" : $"\t\tvar result = {safeMethodName}(")
                .Append(
                    string.Join(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            func.ArgumentNames?[index] ?? $"arg{index + 1}")))
                .AppendLine(");");
            if (returnType == typeof(void))
                builder.AppendLine("\t\treturn IntPtr.Zero;");
            else
                builder.Append(
                        "\t\tif (result == null)\n\t\t{\n\t\t\treturn IntPtr.Zero;\n\t\t}\n\n\t\treturn ServerFunction.GetDatum((")
                    .Append(returnType.FullName)
                    .AppendLine(")result);");
            builder.AppendLine("\t}")
                .AppendLine()
                .Append("\tprivate static ")
                .Append(returnTypeName)
                .Append(returnType == typeof(void) ? " " : "? ")
                .Append(safeMethodName)
                .Append("(")
                .Append(
                    string.Join(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            $"{ServerTypes.GetTypeForOid(oid).FullName}? {func.ArgumentNames?[index] ?? $"arg{index + 1}"}")))
                .AppendLine(")")
                .AppendLine("\t{")
                .Append("\t\t")
                .AppendLine(func.FunctionBody.Replace("\n", "\n\t\t"))
                .AppendLine("\t}")
                .AppendLine("}");

            var generatedCode = builder.ToString();
            var tree = CSharpSyntaxTree.ParseText(generatedCode);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

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
                    ServerLog.ELog(GetSeverityLevel(resultDiagnostic.Severity), resultDiagnostic.ToString());

                return Assembly.Load(ms.ToArray()).GetType(className)!.GetMethod($"Execute_{safeMethodName}")!.CreateDelegate(typeof(FunctionCallDelegate));
            }
            else
            {
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

                return null;
            }
        }

        private static string GetSafeFunctionName(string functionName)
            => new string(functionName.Take(1).Select(c => char.IsLetter(c) ? c : '_').Concat(functionName.Skip(1).Select(c => char.IsLetterOrDigit(c) ? c : '_')).ToArray());
    }
}
