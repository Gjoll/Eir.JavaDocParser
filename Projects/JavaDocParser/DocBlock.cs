using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eir.JavaDocParser
{
    class DocBlock
    {
        public StringBuilder Text { get; } = new StringBuilder();
        public String Author { get; set; }
        public String Return { get; set; }
        public String Source { get; set; }
        public Dictionary<String, StringBuilder> Params { get; } = new Dictionary<String, StringBuilder>();

        public static DocBlock Parse(String javaDoc)
        {
            String[] lines = javaDoc.Replace("\r", "").Split('\n');
            return Parse(lines);
        }

        public static DocBlock Parse(IEnumerable<String> javaDoc)
        {
            String[] lines = javaDoc.ToArray();
            DocBlock retVal = new DocBlock();
            Int32 i = 0;
            String line;

            bool ParseAttributes()
            {
                String GetWord(String l,
                    out String restOfLine)
                {
                    l = l.Trim();

                    Int32 i = l.IndexOf(" ");
                    if (i < 0)
                    {
                        String retVal = l;
                        restOfLine = String.Empty;
                        return retVal;
                    }

                    {
                        String retVal = l.Substring(0, i);
                        restOfLine = l.Substring(i);
                        return retVal;
                    }
                }
                String blockTagName = GetWord(line, out String restOfLine);

                switch (blockTagName.Trim())
                {
                    case "@author":
                        retVal.Author = restOfLine.Trim();
                        return true;

                    case "@param":
                        //retVal.Author = blockTagContent.ToString();
                        return true;

                    case "@return":
                        retVal.Return = restOfLine.Trim();
                        return true;

                    case "@throws":
                    case "@version":
                    case "@see":
                    case "@since":
                    case "@serial":
                    case "@deprecated":
                        return true;

                    default:
                        return false;
                }
            }

            while (i < lines.Length - 1)
            {
                line = lines[i++].Trim();
                if (line.StartsWith("*/"))
                    return retVal;
                if (line.StartsWith("*"))
                    line = line.Substring(1);
                if (line.Trim().StartsWith("@"))
                {
                    if (ParseAttributes() == false)
                        retVal.Text.AppendLine(line);
                }
                else if ((line.Length > 0) || (retVal.Text.Length > 0))
                    retVal.Text.AppendLine(line);
            }
            return retVal;
        }
    }
}
