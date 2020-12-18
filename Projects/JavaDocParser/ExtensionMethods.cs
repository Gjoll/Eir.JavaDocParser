using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Eir.JavaDocParser
{
    static class ExtensionMethods
    {
        public static String RemoveQuotes(this String name)
        {
            if (name[0] == '"')
                name = name.Substring(1);
            if (name[^1] == '"')
                name = name.Substring(0, name.Length - 1);
            return name;
        }

        public static StringBuilder AppendCommentLines(this StringBuilder sb,
            IEnumerable<String> text)
        {
            foreach (String line in text)
            {
                string s = line.Replace("\r", "");
                sb.AppendLine($"/// {s}");
            }
            return sb;
        }

        public static StringBuilder AppendCommentLines(this StringBuilder sb, 
            StringBuilder text)
        {
            foreach (String line in text.ToString().Split("\n"))
            {
                string s = line.Replace("\r", "");
                sb.AppendLine($"/// {s}");
            }
            return sb;
        }
    }
}
