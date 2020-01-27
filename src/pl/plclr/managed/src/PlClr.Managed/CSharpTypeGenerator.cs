using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Globalization;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PlClr
{
    internal static class CSharpTypeGenerator
    {
        private static readonly SyntaxList<UsingDirectiveSyntax> UsingDirectives = List(
            new[]
            {
                UsingDirective(
                    IdentifierName("System"))
            });

        public static CSharpCompileData GetCompileData(CompositeTypeInfo compositeTypeInfo)
        {
            var oidString = compositeTypeInfo.Oid.ToString(CultureInfo.InvariantCulture);
            var className = "PlClrTypeForOid_" + oidString;
            var methodName = "Get_" + oidString;
            var heapTupleHeaderVariableName = "heapTupleHeader";
            var localPointerVariableSuffix = "Ptr";
            var localBoolVariableSuffix = "IsNull";
            var attributes = compositeTypeInfo.Attributes;
            var length = compositeTypeInfo.Attributes.Length;

            var constructorParameterList = new List<SyntaxNodeOrToken>(length);
            var constructorBodyStatements = new List<StatementSyntax>(length);
            var classMembers = new List<MemberDeclarationSyntax>(length);
            var constructorInvokationList = new List<SyntaxNodeOrToken>(length);
            var localVariableAssignmentList = new List<LocalDeclarationStatementSyntax>(length);

            for (var i = 0; i < length; i++)
            {
                var attribute = attributes[i];
                constructorParameterList.Add(GetConstructorParameter(attribute));
                constructorBodyStatements.Add(GetConstructorBodyStatement(attribute.Name));
                classMembers.Add(GetPropertyDeclaration(attribute));
                constructorInvokationList.Add(GetConstructorInvocationParameter(heapTupleHeaderVariableName, localPointerVariableSuffix, localBoolVariableSuffix, attribute));
                if (!attribute.NotNull)
                {
                    localVariableAssignmentList.Add(GetMethodLocalVariableAssignment(heapTupleHeaderVariableName, localPointerVariableSuffix, localBoolVariableSuffix, attribute));
                }
                if (i + 1 < length)
                {
                    constructorParameterList.Add(Token(SyntaxKind.CommaToken));
                    constructorInvokationList.Add(Token(SyntaxKind.CommaToken));
                }

            }

            classMembers.Insert(
                0,
                GetConstructorDeclaration(
                    className,
                    ParameterList(Token(SyntaxKind.OpenParenToken), SeparatedList<ParameterSyntax>(constructorParameterList), Token(SyntaxKind.CloseParenToken)),
                    constructorBodyStatements)
            );

            var methodArgumentName = "datum";
            classMembers.Add(GetMethodDeclaration(className, methodName, methodArgumentName, GetMethodBodyStatements(className, heapTupleHeaderVariableName, methodArgumentName, localVariableAssignmentList, constructorInvokationList)));

            var compilatonUnit = CompilationUnit(
                    default,
                    UsingDirectives,
                    default,
                    SingletonList<MemberDeclarationSyntax>(
                        GetNamespaceDeclaration(
                            GetClassDeclaration(className, classMembers))),
                    Token(SyntaxKind.EndOfFileToken))
                .NormalizeWhitespace();
            return new CSharpCompileData(className, methodName, compilatonUnit);
        }

        private static LocalDeclarationStatementSyntax GetMethodLocalVariableAssignment(string heapTupleHeaderVariableName, string localPointerVariableSuffix, string localBoolVariableSuffix, AttributeInfo attribute)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName("var"),
                    SingletonSeparatedList(
                        VariableDeclarator(
                            Identifier(attribute.Name + localPointerVariableSuffix),
                            default,
                            EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(nameof(ServerFunctions)),
                                        IdentifierName(nameof(ServerFunctions.GetAttributeByNum))),
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(
                                                    IdentifierName(heapTupleHeaderVariableName)),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(attribute.RowNumber))),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(default, Token(SyntaxKind.OutKeyword),
                                                    DeclarationExpression(
                                                        IdentifierName("var"),
                                                        SingleVariableDesignation(
                                                            Identifier(attribute.Name +
                                                                       localBoolVariableSuffix))))
                                            })))
                            ))))
            );
        }

        private static ArgumentSyntax GetConstructorInvocationParameter(string heapTupleHeaderVariableName, string localPointerVariableSuffix, string localBoolVariableSuffix, AttributeInfo attribute)
        {
            var attributeTypeAccessInfo = ServerTypes.GeTypeAccessInfo(attribute.Type.Oid);
            if (attribute.NotNull)
                return Argument(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(attributeTypeAccessInfo.AccessMethodType.Name),
                            IdentifierName(attributeTypeAccessInfo.AccessMethodName)),
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(nameof(ServerFunctions)),
                                            IdentifierName(nameof(ServerFunctions.GetAttributeByNum))),
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    Argument(IdentifierName(heapTupleHeaderVariableName)),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                                        Literal(attribute.RowNumber))),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(default, Token(SyntaxKind.OutKeyword), ParseExpression("_"))
                                                }))))))));
            return Argument(
                ConditionalExpression(
                    IdentifierName(attribute.Name + localBoolVariableSuffix),
                    attributeTypeAccessInfo.MappedType.IsValueType
                        ? (ExpressionSyntax) CastExpression(GetTypeSyntax(attribute.Type.Oid, false), LiteralExpression(SyntaxKind.NullLiteralExpression))
                        : LiteralExpression(SyntaxKind.NullLiteralExpression),
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(attributeTypeAccessInfo.AccessMethodType.Name),
                            IdentifierName(attributeTypeAccessInfo.AccessMethodName)),
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(IdentifierName(attribute.Name + localPointerVariableSuffix))))
                    )));
        }

        private static MemberDeclarationSyntax GetMethodDeclaration(string className, string methodName, string methodArgumentName, SyntaxList<StatementSyntax> methodBodyStatements)
        {
            return MethodDeclaration(
                default,
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
                IdentifierName(className),
                default,
                Identifier(methodName),
                default,
                ParameterList(SingletonSeparatedList(Parameter(default, default, IdentifierName("IntPtr"), Identifier(methodArgumentName), default))),
                default,
                Block(methodBodyStatements),
                default,
                default);
        }

        private static SyntaxList<StatementSyntax> GetMethodBodyStatements(string className, string heapTupleHeaderVariableName, string methodArgumentName, List<LocalDeclarationStatementSyntax> localVariableAssignmentList, List<SyntaxNodeOrToken> constructorInvokationList)
        {
            var statements = new List<StatementSyntax>
            {
                LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier(heapTupleHeaderVariableName))
                                .WithInitializer(
                                    EqualsValueClause(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(nameof(ServerFunctions)),
                                                    IdentifierName(nameof(ServerFunctions.DeToastDatum))))
                                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName(methodArgumentName))))))))))
            };
            statements.AddRange(localVariableAssignmentList);

            statements.Add(LocalDeclarationStatement(
                                        VariableDeclaration(
                                            IdentifierName("var"))
                                        .WithVariables(
                                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                VariableDeclarator(
                                                    Identifier("ret"))
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        ObjectCreationExpression(
                                                            IdentifierName(className))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SeparatedList<ArgumentSyntax>(
                                                                    constructorInvokationList)))))))));
            statements.Add(IfStatement(
                BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    IdentifierName(heapTupleHeaderVariableName),
                    IdentifierName(methodArgumentName)),
                ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(nameof(ServerMemory)),
                                IdentifierName(nameof(ServerMemory.PFree))),
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        IdentifierName(heapTupleHeaderVariableName)))))
                        )));
            statements.Add(ReturnStatement(IdentifierName("ret")));

            return List(statements);
        }

        private static NamespaceDeclarationSyntax GetNamespaceDeclaration(ClassDeclarationSyntax classDeclaration)
        {

            return NamespaceDeclaration(
                default,
                default,
                Token(SyntaxKind.NamespaceKeyword),
                IdentifierName(nameof(PlClr)),
                Token(SyntaxKind.OpenBraceToken),
                default,
                default,
                SingletonList<MemberDeclarationSyntax>(classDeclaration),
                Token(SyntaxKind.CloseBraceToken),
                default);
        }

        private static ClassDeclarationSyntax GetClassDeclaration(string className, IEnumerable<MemberDeclarationSyntax> members)
        {
            return ClassDeclaration(default,
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword)),
                Token(SyntaxKind.ClassKeyword),
                Identifier(className),
                default,
                default,
                default,
                Token(SyntaxKind.OpenBraceToken),
                List(members),
                Token(SyntaxKind.CloseBraceToken),
                default
            );
        }

        private static MemberDeclarationSyntax GetConstructorDeclaration(string className, ParameterListSyntax parameters, IEnumerable<StatementSyntax> bodyStatements)
        {
            return ConstructorDeclaration(
                default,
                TokenList(Token(SyntaxKind.PublicKeyword)),
                Identifier(className),
                parameters,
                default,
                Block(bodyStatements),
                default,
                default
                );
        }

        private static ParameterSyntax GetConstructorParameter(AttributeInfo attribute)
        {
            return Parameter(default, default, GetTypeSyntax(attribute.Type.Oid, attribute.NotNull), Identifier(attribute.Name), default);
        }

        private static StatementSyntax GetConstructorBodyStatement(string attributeName)
        {
            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(attributeName)),
                    IdentifierName(attributeName)));
        }

        private static MemberDeclarationSyntax GetPropertyDeclaration(AttributeInfo attribute)
            => PropertyDeclaration(
                default,
                TokenList(Token(SyntaxKind.PublicKeyword)),
                GetTypeSyntax(attribute.Type.Oid, attribute.NotNull),
                default,
                Identifier(attribute.Name),
                AccessorList(
                    List(
                        new[]
                        {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        })),
                default,
                default,
                default);

        private static TypeSyntax GetTypeSyntax(uint typeOid, bool notNull)
        {
            TypeSyntax baseTypeSyntax;
            switch (typeOid)
            {
                case 23:
                    baseTypeSyntax = PredefinedType(Token(SyntaxKind.IntKeyword));
                    break;
                case 25:
                    baseTypeSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));
                    break;
                default:
                    var parts = ServerTypes.GeTypeAccessInfo(typeOid).MappedType.FullName!.Split('.');
                    if (parts.Length == 1)
                    {
                        baseTypeSyntax = IdentifierName(parts[0]);
                        break;
                    }

                    var nameSyntax = QualifiedName(IdentifierName(parts[1]), IdentifierName(parts[2]));
                    for (var i = 3; i < parts.Length; i++)
                    {
                        nameSyntax = QualifiedName(nameSyntax, IdentifierName(parts[i]));
                    }
                    baseTypeSyntax = nameSyntax;
                    break;
            }

            return notNull ? baseTypeSyntax : NullableType(baseTypeSyntax);
        }

    }
}
