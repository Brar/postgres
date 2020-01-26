using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using static PlClr.Globals;

namespace PlClr
{
    internal static class CSharpCompiler
    {
        private static readonly List<MetadataReference> MetadataReferences;

        static CSharpCompiler()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            MetadataReferences = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(PlClrMain).Assembly.Location)
            };
        }

        public static FunctionCallDelegate Compile(FunctionCompileInfo func)
        {
            // We can't compile if PlClrMain did not complete Setup
            Debug.Assert(BackendFunctions != null);

            var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(CSharpCompiler).Assembly)!;
            var assemblyName = $"PlClrAssembly_{func.FunctionOid}";
            var className = $"PlClrClass_{func.FunctionOid}";
            var safeMethodName = GetSafeFunctionName(func.FunctionName);
            var assembly = assemblyLoadContext.Assemblies.FirstOrDefault(a => a.GetName().Name == assemblyName) ??
                           CompileAssembly(func, assemblyLoadContext, assemblyName, className, safeMethodName);

            return (FunctionCallDelegate)assembly.GetType(className)!.GetMethod($"Execute_{safeMethodName}")!.CreateDelegate(typeof(FunctionCallDelegate));
        }

        private static Assembly CompileAssembly(FunctionCompileInfo func, AssemblyLoadContext assemblyLoadContext, string assemblyName, string className, string safeMethodName)
        {
            var returnType = ServerTypes.GetTypeForOid(func.ReturnValueType);
            var returnTypeName = returnType == typeof(void) ? "void" : returnType.FullName;
            var builder = new StringBuilder()
                .Append($"using {nameof(PlClr)};\n")
                .Append("using System;\n\n")
                .Append("using static PlClr.Globals;\n\n")
                .Append("public static class ")
                .Append(className)
                .Append("\n{\n")
                .Append("\tpublic static IntPtr Execute_")
                .Append(safeMethodName)
                .Append('(')
                .Append(nameof(NullableDatum))
                .Append("[] values)\n")
                .Append("\t{\n");

            if (func.ArgumentOids.Any())
            {
                builder.Append("\t\t");
                if (func.IsStrict)
                {
                    builder.AppendJoin($"\n\t\t",
                        func.ArgumentOids.Select((oid, index) =>
                        $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = {nameof(BackendFunctions)}.{ServerTypes.GetValueAccessMethodForOid(oid)}(values[{index}].Value);"
                        ));
                }
                else
                {
                    builder.AppendJoin($"\n\t\t",
                            func.ArgumentOids.Select((oid, index) =>
                                $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = values[{index}].{nameof(NullableDatum.IsNull)} ? ({ServerTypes.GetTypeForOid(oid).FullName}?)null : {nameof(BackendFunctions)}.{ServerTypes.GetValueAccessMethodForOid(oid)}(values[{index}].{nameof(NullableDatum.Value)});"));
                }
                builder.Append("\n\n");
            }
            builder.Append(returnType == typeof(void) ? $"\t\t{safeMethodName}(" : $"\t\tvar result = {safeMethodName}(")
                .AppendJoin(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            func.ArgumentNames?[index] ?? $"arg{index + 1}"))
                .Append(");\n\t\t")
                .Append(nameof(NullableDatum))
                .Append(" returnValue;\n\t\t");
            if (returnType == typeof(void))
            {
                builder.Append("returnValue.")
                    .Append(nameof(NullableDatum.IsNull))
                    .Append(" = false;\n\t\t")
                    .Append("returnValue.")
                    .Append(nameof(NullableDatum.Value))
                    .Append(" = IntPtr.Zero;\n\n\t\t")
                    .Append("return ")
                    .Append(nameof(Marshal))
                    .Append('.')
                    .Append(nameof(Marshal.StructureToPtrPalloc))
                    .Append("(returnValue);\n");
            }
            else
            {
                if (!func.IsStrict)
                    builder.Append("if (result == null)\n")
                        .Append("\t\t{\n")
                        .Append("\t\t\treturnValue.")
                        .Append(nameof(NullableDatum.IsNull))
                        .Append(" = true;\n")
                        .Append("\t\t\treturnValue.")
                        .Append(nameof(NullableDatum.Value))
                        .Append(" = IntPtr.Zero;\n")
                        .Append("\t\t}\n")
                        .Append("\t\telse\n")
                        .Append("\t\t{\n")
                        .Append("\t\t\treturnValue.")
                        .Append(nameof(NullableDatum.IsNull))
                        .Append(" = false;\n")
                        .Append("\t\t\treturnValue.")
                        .Append(nameof(NullableDatum.Value))
                        .Append(" = ")
                        .Append(nameof(BackendFunctions))
                        .Append('.')
                        .Append(ServerTypes.GetValueCreationMethodForOid(func.ReturnValueType))
                        .Append("((")
                        .Append(returnType.FullName)
                        .Append(")result);\n")
                        .Append("\t\t}\n\n");
                else
                    builder.Append("returnValue.")
                        .Append(nameof(NullableDatum.IsNull))
                        .Append(" = false;\n\t\t")
                        .Append("returnValue.")
                        .Append(nameof(NullableDatum.Value))
                        .Append(" = ")
                        .Append(nameof(BackendFunctions))
                        .Append('.')
                        .Append(ServerTypes.GetValueCreationMethodForOid(func.ReturnValueType))
                        .Append("(result);\n\n");

                builder.Append(
                        "\t\treturn ")
                    .Append(nameof(Marshal))
                    .Append('.')
                    .Append(nameof(Marshal.StructureToPtrPalloc))
                    .Append("(returnValue);\n");
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
            var tree = CSharpSyntaxTree.ParseText(generatedCode);
            ServerLog.ELog(SeverityLevel.Debug1, $"PL/CLR generated code:\n{tree}");

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);

            var compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees: new[] { tree }, references: MetadataReferences,
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

                return assemblyLoadContext.LoadFromStream(ms);
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

        public static string CreateValueAccessMethod(TypeInfo typeInfo)
        {
            if (typeInfo is CompositeTypeInfo compositeTypeInfo)
            {
                var assemblyName = $"PlClrAssembly_{typeInfo.Oid}";
                var typeSyntaxTree = CSharpTypeGenerator.GetCompilationUnit(compositeTypeInfo, out string methodName).SyntaxTree;
                
                ServerLog.ELog(SeverityLevel.Debug1, $"PL/CLR generated code:\n{typeSyntaxTree}");

                var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);

                var compilation = CSharpCompilation.Create(assemblyName,
                    syntaxTrees: new[] { typeSyntaxTree }, references: MetadataReferences,
                    options: options);

                var ms = new MemoryStream();
                var res = compilation.Emit(ms);
                ms.Seek(0L, SeekOrigin.Begin);
                var ms2 = new MemoryStream(ms.ToArray());
                ms2.Seek(0L, SeekOrigin.Begin);

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

                    MetadataReferences.Add(MetadataReference.CreateFromStream(ms));
                    AssemblyLoadContext.GetLoadContext(typeof(CSharpCompiler).Assembly)!.LoadFromStream(ms2);
                    return methodName;
                }
            }
            throw new NotImplementedException();
        }
    }
}
