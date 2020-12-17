using System;
using System.Collections.Generic;
using System.Text;

namespace Eir.JavaDocParser
{
    class DocBlock
    {
        public StringBuilder Text { get; } = new StringBuilder();
        public String Author { get; set; }
        public String Return { get; set; }
        public String Source { get; set; }
        public Dictionary<String, StringBuilder> Params { get; } = new Dictionary<String, StringBuilder>();
    }
}
