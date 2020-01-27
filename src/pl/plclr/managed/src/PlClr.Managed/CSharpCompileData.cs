using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlClr
{
    internal class CSharpCompileData
    {
        public CSharpCompileData(string className, string methodName, CompilationUnitSyntax compilationUnit)
        {
            ClassName = className;
            MethodName = methodName;
            CompilationUnit = compilationUnit;
        }

        public string ClassName { get; }
        public string MethodName { get; }
        public CompilationUnitSyntax CompilationUnit { get; }
    }
}
