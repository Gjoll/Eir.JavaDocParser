using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Antlr4.Runtime;
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

            s = String.Empty;
            return retVal;
        }

        DocBlock ParseDocBlock(String javaFilePath,
            String line)
        {
            bool IsEnd()
            {
                line = line.Trim();
                if (line.Trim().EndsWith("*/") == false)
                    return false;
                line = line.Substring(line.Length - 2);
                return true;
            }

            StringBuilder docLines = new StringBuilder();
            docLines.AppendLine(line);
            bool end = IsEnd();
            while (end == false)
            {
                line = lines[this.lineIndex++];
                end = IsEnd();
                if (line.Length > 0)
                    docLines.AppendLine(line);
            }
            return DocBlock.Parse(docLines.ToString());
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
