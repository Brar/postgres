using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlClr
{
    internal static class CSharpCompiler
    {
        public static MethodInfo? Compile(FunctionCompileInfo func)
        {
            var tree = CSharpSyntaxTree.ParseText(@$"
public static class _{func.FunctionOid}_
{{
    public static void {func.FunctionName}()
    {{
        {func.FunctionBody}
    }}   
}}");

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("Test1",
                syntaxTrees: new[] { tree }, references: new[] { mscorlib },
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
                    .OrderBy(d=> d.Location.IsInSource ? d.Location.SourceSpan.Start : int.MaxValue))
                    ServerLog.ELog(GetSeverityLevel(resultDiagnostic.Severity), resultDiagnostic.ToString());

                return Assembly.Load(ms.ToArray()).GetType($"_{func.FunctionOid}_")!.GetMethod(func.FunctionName);

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
                    ServerLog.ELog(SeverityLevel.Info, infos);

                var warnings = GetDiagnostics(res, DiagnosticSeverity.Warning);
                if (warnings.Length > 0)
                    ServerLog.ELog(SeverityLevel.Warning, warnings);

                var errors = GetDiagnostics(res, DiagnosticSeverity.Error);
                if (errors.Length > 0)
                    ServerLog.ELog(SeverityLevel.Error, errors);

                return null;
            }
        }
    }
}
