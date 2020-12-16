using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Eir.JavaDocParser
{
    class DocBlock
    {
        public List<String> Text { get; } = new List<string>();
    }

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

        DocBlock ParseDocBlock(String line)
        {
            DocBlock retVal = new DocBlock();
            retVal.Text.Add(line.Trim());
            while (this.lineIndex < lines.Length)
            {
                line = lines[this.lineIndex++];
                String hdr = LineHdr(ref line);
                retVal.Text.Add(line.Trim());
                switch (hdr)
                {
                    case "*/":
                        return retVal;
                }
            }

            return retVal;
        }

        public override void ParseFile(String javaFile)
        {
            lastBlock = null;
            lines = File.ReadAllLines(javaFile);
            this.lineIndex = 0;

            while (this.lineIndex < lines.Length)
            {
                String line = lines[this.lineIndex++];
                String hdr = LineHdr(ref line);

                switch (hdr)
                {
                    case "/**":
                        lastBlock = ParseDocBlock(line);
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
