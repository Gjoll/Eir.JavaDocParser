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
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;

namespace Eir.JavaDocParser
{
    class CSParser : ParserBase
    {
        public override String FileExtension => "*.cs";
        private JavaParser javaParser;
        private List<String> csFiles = new List<string>();

        public DocBlock GetBlock(String name)
        {
            name = name.RemoveQuotes();
            if (this.javaParser.Docs.TryGetValue(name, out DocBlock block) == false)
                return null;
            return block;
        }

        public override void ParseFile(String path)
        {
            csFiles.Add(path);
        }


        void Normalize(String fileName, SyntaxTree syntaxTree)
        {
            CSRewriter csRewriter = new CSRewriter(this);
            SyntaxNode result = csRewriter.Visit(syntaxTree.GetRoot());
            using (var workspace = new AdhocWorkspace())
            {
                OptionSet options = workspace.Options;
                options = options.WithChangedOption(CSharpFormattingOptions.SpacingAroundBinaryOperator, BinaryOperatorSpacingOptions.Ignore);
                result = Formatter.Format(result, workspace, options);
            }

            File.WriteAllText(fileName, result.ToFullString());
        }


        public void Normalize(JavaParser javaParser)
        {
            this.javaParser = javaParser;
            foreach (String fileName in this.csFiles)
            {
                String csText = File.ReadAllText(fileName);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(csText);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                this.Normalize(fileName, tree);
            }
        }
    }
}
