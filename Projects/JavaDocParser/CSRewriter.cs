using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Eir.JavaDocParser
{
    class CSRewriter : CSharpSyntaxRewriter
    {
        private CSParser csParser;

        public CSRewriter(CSParser csParser)
        {
            this.csParser = csParser;
        }

        bool JavaAttribute(SyntaxList<AttributeListSyntax> atListList,
            out String name)
        {
            name = null;
            foreach (AttributeListSyntax atList in atListList)
            {
                foreach (AttributeSyntax at in atList.Attributes)
                {
                    var attributeName = at.Name.NormalizeWhitespace().ToFullString();
                    if (attributeName == "JavaAttribute")
                    {
                        name = at
                            .ArgumentList
                            .Arguments
                            .First()
                            .Expression
                            .GetFirstToken()
                            .Text;

                        name = name.RemoveQuotes();

                        return true;
                    }
                }
            }

            return false;
        }

        bool ProcessJavaAttribute(SyntaxList<AttributeListSyntax> attListLists,
            ref SyntaxTriviaList sf)
        {
            if (JavaAttribute(attListLists, out String argumentValue) == false)
                return false;

            DocBlock docBlock = this.csParser.GetBlock(argumentValue);
            if (docBlock == null)
            {
                Trace.WriteLine($"{argumentValue} contains no documentation");
                return false;
            }

            List<String> textLines = new List<string>(docBlock.Text.ToString().Replace("\r", "").Split("\n"));
            while ((textLines.Count > 0) && (String.IsNullOrWhiteSpace(textLines[0])))
                textLines.RemoveAt(0);
            while ((textLines.Count > 0) && (String.IsNullOrWhiteSpace(textLines[textLines.Count - 1])))
                textLines.RemoveAt(textLines.Count - 1);
            if (textLines.Count == 0)
                return false;

            StringBuilder sb = new StringBuilder();
            sb
                .AppendLine("/// <summary>")
                .AppendCommentLines(textLines)
                .AppendLine("/// </summary>")
                ;
            //sf = SyntaxFactory.Comment(sb.ToString());

            return true;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            SyntaxNode retVal = base.VisitInterfaceDeclaration(node);

            SyntaxTriviaList sf = retVal.GetLeadingTrivia();
            if (this.ProcessJavaAttribute(node.AttributeLists, ref sf) == false)
                return retVal;

            retVal = retVal.WithLeadingTrivia(sf);
            return retVal;
        }

        //public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        //{
        //    // determination of the type of the variable(s)
        //    var typeSymbol = (ITypeSymbol)this.SemanticModel.GetSymbolInfo(node.Type).Symbol;

        //    bool changed = false;

        //    // you could declare more than one variable with one expression
        //    SeparatedSyntaxList<VariableDeclaratorSyntax> vs = node.Variables;
        //    // we create a space to improve readability
        //    SyntaxTrivia space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");

        //    for (var i = 0; i < node.Variables.Count; i++)
        //    {
        //        // there is not an initialization
        //        if (this.SemanticModel.GetSymbolInfo(node.Type).Symbol.ToString() == "int" &&
        //            node.Variables[i].Initializer == null)
        //        {
        //            // we create a new espression "42"
        //            // preceded by the space we create earlier
        //            ExpressionSyntax es = SyntaxFactory.ParseExpression("42")
        //                                               .WithLeadingTrivia(space);

        //            // basically we create an assignment to the espression we just created
        //            EqualsValueClauseSyntax evc = SyntaxFactory.EqualsValueClause(es)
        //                                                       .WithLeadingTrivia(space);

        //            // we replace the null initializer with ours
        //            vs = vs.Replace(vs.ElementAt(i), vs.ElementAt(i).WithInitializer(evc));

        //            changed = true;
        //        }

        //        // there is an initialization but it's not to 42
        //        if (this.SemanticModel.GetSymbolInfo(node.Type).Symbol.ToString() == "int" &&
        //            node.Variables[i].Initializer != null &&
        //            !node.Variables[i].Initializer.Value.IsEquivalentTo(SyntaxFactory.ParseExpression("42")))
        //        {
        //            ExpressionSyntax es = SyntaxFactory.ParseExpression("42")
        //                                               .WithLeadingTrivia(space);

        //            EqualsValueClauseSyntax evc = SyntaxFactory.EqualsValueClause(es);

        //            vs = vs.Replace(vs.ElementAt(i), vs.ElementAt(i).WithInitializer(evc));

        //            changed = true;
        //        }
        //    }

        //    if (changed == true)
        //    {
        //        return node.WithVariables(vs);
        //    }

        //    return base.VisitVariableDeclaration(node);
        //}
    }
}
