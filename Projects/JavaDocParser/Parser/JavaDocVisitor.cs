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
        public bool DebugFlag { get; set; } = false;
        public JavaDocVisitor()
        {
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

        //public override object VisitDocument(CommandParser.DocumentContext context)
        //{
        //    const String fcn = "VisitDocument";
        //    this.TraceMsg(context, fcn);
        //    this.VisitChildren(context);
        //    return null;
        //}

    }
}
