using Antlr4.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Eir.JavaDocParser.Parser;

namespace Eir.JavaDocParser
{
    class CSParser : ParserBase
    {
        public override String FileExtension => "*.cs";
        private JavaParser javaParser;
        private Dictionary<String, SyntaxTree> syntaxTrees = new Dictionary<string, SyntaxTree>();

        String RemoveQuotes(String name)
        {
            if (name[0] == '"')
                name = name.Substring(1);
            if (name[^1] == '"')
                name = name.Substring(0, name.Length - 1);
            return name;
        }

        DocBlock GetBlock(String name)
        {
            name = RemoveQuotes(name);
            if (this.javaParser.Docs.TryGetValue(name, out DocBlock block) == false)
                return null;
            return block;
        }

        public override void ParseFile(String path)
        {
            String csText = File.ReadAllText(path); 
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            syntaxTrees.Add(path, tree);
        }

        void Normalize(AttributeListSyntax att,
            InterfaceDeclarationSyntax interfaceParent)
        {
            Trace.WriteLine($"Interface {interfaceParent.Identifier.Text}");

            var firstAttribute = att.Attributes.First();
            var attributeName = firstAttribute.Name.NormalizeWhitespace().ToFullString();

            var firstArgument = firstAttribute
                .ArgumentList
                .Arguments
                .First()
                .Expression
                .GetFirstToken()
                .Text;

            firstArgument = RemoveQuotes(firstArgument);
            DocBlock docBlock = this.GetBlock(firstArgument);
            if (docBlock == null)
            {
                Trace.WriteLine($"{firstArgument} contains no documentation");
                return;
            }
        }
        

        void Normalize(AttributeListSyntax att)
        {
            switch (att.Parent)
            {
                case InterfaceDeclarationSyntax interfaceParent:
                    Normalize(att, interfaceParent);
                    break;
                
                case ClassDeclarationSyntax classParent:
                    break;
                
                case RecordDeclarationSyntax recordParent:
                    break;
                
                case MethodDeclarationSyntax methodParent:
                    break;
                
                case PropertyDeclarationSyntax methodParent:
                    break;

                default:
                    throw new NotImplementedException($"Unknown attribute parent type {att.Parent.GetType().Name}");
            }
        }

        void Normalize(SyntaxTree syntaxTree)
        {
            List<AttributeListSyntax> attList = syntaxTree.GetRoot()
                .DescendantNodes().OfType<AttributeListSyntax>()
                .Where(curr => curr.Attributes.Any(currentAttribute => currentAttribute.Name.GetText().ToString() == "JavaAttribute"))
                .ToList();
            foreach (AttributeListSyntax att in attList)
                this.Normalize(att);
        }

       
        public void Normalize(JavaParser javaParser)
        {
            this.javaParser = javaParser;
            foreach (String fileName in this.syntaxTrees.Keys)
            {
                SyntaxTree syntaxTree = this.syntaxTrees[fileName];
                this.Normalize(syntaxTree);
            }
        }
    }
}
