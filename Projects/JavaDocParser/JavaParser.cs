using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Antlr4.Runtime;
using Eir.JavaDocParser.Parser;
using Microsoft.VisualBasic.CompilerServices;

namespace Eir.JavaDocParser
{
    class JavaParser : ParserBase
    {
        public override String FileExtension => "*.java";

        DocBlock lastBlock;
        String[] lines;
        Int32 lineIndex = 0;

        public Dictionary<String, DocBlock> Docs { get; } =
            new Dictionary<string, DocBlock>();

        void StartClass(string type, String name)
        {
            if (this.lastBlock != null)
            {
                this.Docs.Add(name, this.lastBlock);
                this.lastBlock = null;
            }
        }

        String LineHdr(ref String s)
        {
            String word = GetWord(ref s);
            switch (word)
            {
                case "public":
                case "protected":
                case "private":
                    word = GetWord(ref s);
                    break;

            }

            return word;
        }

        String GetWord(ref String s)
        {
            s = s.Trim();
            Int32 index = s.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' });
            String retVal = s;
            if (index > 0)
            {
                retVal = s.Substring(0, index);
                s = s.Substring(index).Trim();
            }

            return retVal;
        }

        DocBlock ParseDocBlock(String javaFilePath,
            String line)
        {
            line = line.Substring(3).Trim();
            if (line.Length > 0)
                return null;
            
            StringBuilder docLines = new StringBuilder();
            bool doneFlag = false;
            while (doneFlag == false)
            {
                line = lines[this.lineIndex++];
                String hdr = LineHdr(ref line);
                docLines.AppendLine(line.Trim());
                switch (hdr)
                {
                    case "*/":
                        doneFlag = true;
                        break;
                }
            }

            String docText = docLines.ToString()
                .Trim()
                .Replace("\r", "");
            String[] inputLines = docText.Split('\n');

            Parser.JavadocLexer lexer = new Parser.JavadocLexer(new AntlrInputStream(docText));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new LocalErrorListenerLexer("JavaDoc Lexer",
                javaFilePath,
                inputLines));

            Parser.JavadocParser parser = new Parser.JavadocParser(new CommonTokenStream(lexer));
            parser.Trace = false;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new LocalErrorListenerParser("JavaDoc Parser",
                javaFilePath,
                inputLines));

            Parser.JavaDocVisitor visitor = new Parser.JavaDocVisitor(javaFilePath);
            visitor.Visit(parser.documentation());
            return visitor.block;
        }

        public override void ParseFile(String javaFilePath)
        {
            lastBlock = null;
            lines = File.ReadAllLines(javaFilePath);
            this.lineIndex = 0;

            while (this.lineIndex < lines.Length)
            {
                String line = lines[this.lineIndex++];
                String hdr = LineHdr(ref line);

                switch (hdr)
                {
                    case "/**":
                        lastBlock = ParseDocBlock(javaFilePath, line);
                        break;

                    case "interface":
                    case "class":
                    case "record":
                        StartClass(hdr, GetWord(ref line));
                        break;

                    default:
                        break;

                }
            }
        }

    }
}
