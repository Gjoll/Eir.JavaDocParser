using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Eir.JavaDocParser
{
    public abstract class ParserBase
    {
        public abstract String FileExtension { get; }

        public List<String> InputDirs = new List<String>();
        public void Parse()
        {
            foreach (String inputDir in this.InputDirs)
                this.ParseDir(inputDir);
        }

        public void ParseDir(String path)
        {
            foreach (String file in Directory.GetFiles(path, FileExtension))
                ParseFile(file);

            foreach (String subDir in Directory.GetDirectories(path))
                ParseDir(subDir);
        }

        public abstract void ParseFile(String path);
    }
}
