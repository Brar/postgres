using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Reflection;

namespace PlClr
{
    internal static class CSharpCompiler
    {
        public static Assembly Compile(FunctionCompileInfo func)
        {
            var tree = CSharpSyntaxTree.ParseText(@$"
public static class _{func.FunctionOid}_
{{
    public static int {func.FunctionName}()
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
            var result = compilation.Emit(ms);

            if (result.Success)
                return Assembly.Load(ms.ToArray());

            throw new Exception(result.ToString());
        }
    }
}
