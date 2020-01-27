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
            //var returnType = ServerTypes.GetTypeForOid(func.ReturnValueType);
            var returnTypeAccessInfo = ServerTypes.GeTypeAccessInfo(func.ReturnValueType);
            var returnsVoid = returnTypeAccessInfo.MappedType == typeof(void);
            var returnTypeName = returnsVoid ? "void" : returnTypeAccessInfo.MappedType.FullName;
            var builder = new StringBuilder()
                .Append($"using {nameof(PlClr)};\n")
                .Append("using System;\n\n")
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
                        {
                            var argumentTypeAccessInfo = ServerTypes.GeTypeAccessInfo(oid);
                            return $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = {argumentTypeAccessInfo.AccessMethodType.Name}.{argumentTypeAccessInfo.AccessMethodName}(values[{index}].Value);";
                        }));
                }
                else
                {
                    builder.AppendJoin($"\n\t\t",
                            func.ArgumentOids.Select((oid, index) =>
                            {
                                var argumentTypeAccessInfo = ServerTypes.GeTypeAccessInfo(oid);
                                return $"var {func.ArgumentNames?[index] ?? $"arg{index + 1}"} = values[{index}].{nameof(NullableDatum.IsNull)} ? ({argumentTypeAccessInfo.MappedType.FullName}?)null : {argumentTypeAccessInfo.AccessMethodType.Name}.{argumentTypeAccessInfo.AccessMethodName}(values[{index}].{nameof(NullableDatum.Value)});";
                            }));
                }
                builder.Append("\n\n");
            }
            builder.Append(returnsVoid ? $"\t\t{safeMethodName}(" : $"\t\tvar result = {safeMethodName}(")
                .AppendJoin(", ",
                        func.ArgumentOids.Select((oid, index) =>
                            func.ArgumentNames?[index] ?? $"arg{index + 1}"))
                .Append(");\n\t\t")
                .Append(nameof(NullableDatum))
                .Append(" returnValue;\n\t\t");
            if (returnsVoid)
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
                        .Append(returnTypeAccessInfo.CreationMethodType.FullName)
                        .Append('.')
                        .Append(returnTypeAccessInfo.CreationMethodName)
                        .Append("((")
                        .Append(returnTypeAccessInfo.MappedType.FullName)
                        .Append(")result);\n")
                        .Append("\t\t}\n\n");
                else
                    builder.Append("returnValue.")
                        .Append(nameof(NullableDatum.IsNull))
                        .Append(" = false;\n\t\t")
                        .Append("returnValue.")
                        .Append(nameof(NullableDatum.Value))
                        .Append(" = ")
                        .Append(returnTypeAccessInfo.CreationMethodType.FullName)
                        .Append('.')
                        .Append(returnTypeAccessInfo.CreationMethodName)
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
                .Append(returnsVoid || func.IsStrict ? " " : "? ")
                .Append(safeMethodName)
                .Append("(")
                .AppendJoin(", ",
                        func.ArgumentOids.Select((oid, index) =>
                        {
                            var argumentTypeAccessInfo = ServerTypes.GeTypeAccessInfo(oid);
                            return $"{argumentTypeAccessInfo.MappedType.FullName}{(func.IsStrict ? string.Empty : "?")} {func.ArgumentNames?[index] ?? $"arg{index + 1}"}";
                        }))
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

            throw ReportCompilationFailure(res);
        }

        private static PlClrUnreachableException ReportCompilationFailure(EmitResult res)
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
            Debug.WriteLine(errors);
            return ServerLog.ELog(SeverityLevel.Error, errors)!;
        }

        private static string GetSafeFunctionName(string functionName)
            => new string(functionName.Take(1).Select(c => char.IsLetter(c) ? c : '_').Concat(functionName.Skip(1).Select(c => char.IsLetterOrDigit(c) ? c : '_')).ToArray());

        internal static TypeAccessInfo CreateTypeAccessInfo(TypeInfo typeInfo)
        {
            if (typeInfo is CompositeTypeInfo compositeTypeInfo)
            {
                var assemblyName = $"PlClrAssembly_{typeInfo.Oid}";
                var compileData = CSharpTypeGenerator.GetCompileData(compositeTypeInfo);
                var typeSyntaxTree = compileData.CompilationUnit.SyntaxTree;
                
                ServerLog.EReport(SeverityLevel.Debug1, $"PL/CLR generated code for composite type '{compositeTypeInfo.Namespace}.{compositeTypeInfo.Name}' (Oid: {compositeTypeInfo.Oid}):\n{typeSyntaxTree}", errorDataType: typeInfo.Oid);

                var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);

                var compilation = CSharpCompilation.Create(assemblyName,
                    syntaxTrees: new[] { typeSyntaxTree }, references: MetadataReferences,
                    options: options);

                using var ms = new MemoryStream();
                var res = compilation.Emit(ms);

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

                    ms.Seek(0L, SeekOrigin.Begin);
                    var assembly = AssemblyLoadContext.GetLoadContext(typeof(CSharpCompiler).Assembly)!.LoadFromStream(ms);
                    ms.Seek(0L, SeekOrigin.Begin);
                    MetadataReferences.Add(MetadataReference.CreateFromStream(ms));
                    var type = assembly.ExportedTypes.Single();
                    return new TypeAccessInfo(type, type, compileData.MethodName, type, /* Todo: implement value creation method */ null!);
                }
                throw ReportCompilationFailure(res);
            }
            throw ServerLog.EReport(SeverityLevel.Error ,$"Cannot create a TypeAccessInfo for type '{typeInfo.GetType()}'", errorDataType: typeInfo.Oid)!;
        }
    }
}
