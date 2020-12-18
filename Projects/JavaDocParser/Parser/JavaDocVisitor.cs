using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Eir.JavaDocParser.Parser
{
    class JavaDocVisitor : JavadocParserBaseVisitor<Object>
    {
        public DocBlock block;
        
        public bool DebugFlag { get; set; } = false;
        public JavaDocVisitor(String sourcePath)
        {
            block = new DocBlock
            {
                Source = sourcePath
            };
        }

        void TraceMsg(ParserRuleContext context, String fcn)
        {
            if (!this.DebugFlag)
                return;
            String text = context
                .GetText()
                .Replace("\r", "")
                .Replace("\n", "");
            Trace.WriteLine($"{fcn}. '{text}'");
        }

        public override object VisitDescriptionLine(JavadocParser.DescriptionLineContext context)
        {
            String line = context.GetText();
            String trimLine = line.Trim();
            if (trimLine.StartsWith("*"))
                line = trimLine.Substring(1);
            this.block.Text.AppendLine(line);
            return null;
        }

        public override object VisitBlockTag(JavadocParser.BlockTagContext context)
        {
            String blockTagName = context.blockTagName().GetText();
            StringBuilder blockTagContent = new StringBuilder();
            foreach (var content in context.blockTagContent())
                blockTagContent.AppendLine(context.GetText());

            switch (blockTagName.Trim())
            {
                case "JsonUnmarshaler":
                case "JsonMarshaler":
                case "Unmarshaler":
                case "Marshaler":
                case "Override":
                case "throws":
                case "exception":
                case "yahoo":
                    break;

                case "param":
                    //this.block.Author = blockTagContent.ToString();
                    break;

                case "return":
                    this.block.Return = blockTagContent.ToString();
                    break;

                case "see":
                    //this.block.Author = blockTagContent.ToString();
                    break;

                case "author":
                    this.block.Author = blockTagContent.ToString();
                    break;

                default:
                    TraceMsg(context, $"Unknown block tag {blockTagName}");
                    break;
            }
            return null;
        }
    }
}
