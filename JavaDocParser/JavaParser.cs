using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eir.JavaDocParser
{
    class JavaParser
    {
        class DocBlock
        {
            public List<String> Text { get; } = new List<string>();
        }

        public List<String> InputDirs = new List<String>();

        public void Parse()
        {
            foreach (String inputDir in this.InputDirs)
                this.ParseDir(inputDir);
        }

        void ParseFile(String javaFile)
        {
            String[] lines = File.ReadAllLines(javaFile);
            Int32 i = 0;

            String Rem(String s, String r)
            {
                if (s.StartsWith(r))
                    s = s.Substring(r.Length).Trim();
                return s;
            }

            String LineHdr(String s)
            {
                s = s.Trim();
                Rem(s, "public");
                Rem(s, "protected");
                Rem(s, "private");
                Rem(s, "internal");
                Int32 index = s.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' });
                if (index > 0)
                    s = s.Substring(0, index);
                return s;
            }

            DocBlock ParseDocBlock(String line)
            {
                DocBlock retVal = new DocBlock();
                retVal.Text.Add(line.Trim());
                while (i < lines.Length)
                {
                    line = lines[i++];
                    String hdr = LineHdr(line);
                    retVal.Text.Add(line.Trim());
                    switch (hdr)
                    {
                        case "*/":
                            return retVal;
                    }
                }

                return retVal;
            }

            DocBlock lastBlock = null;

            while (i < lines.Length)
            {
                String line = lines[i++];
                String hdr = LineHdr(line);

                switch (hdr)
                {
                    case "/**":
                        lastBlock = ParseDocBlock(line);
                        break;
                }
            }
        }


        public void ParseDir(String path)
        {
            foreach (String file in Directory.GetFiles(path, "*.java"))
                ParseFile(file);

            foreach (String subDir in Directory.GetDirectories(path))
                ParseDir(subDir);
        }

    }
}
