using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eir.JavaDocParser.Parser
{
    class ErrorListener
    {
        String parserName;
        String sourceName;
        private String[] inputLines;

        public ErrorListener(String parserName,
            String sourceName,
            String[] inputLines)
        {
            this.parserName = parserName;
            this.sourceName = sourceName;
            this.inputLines = inputLines;
        }

        public void Error(IRecognizer recognizer,
            Int32 line,
            Int32 charPositionInLine,
            String msg,
            RecognitionException e)
        {
            String msgLine = null;

            if ((line > 0) && (line <= this.inputLines.Length))
            {
                StringBuilder sb = new StringBuilder();
                for (Int32 i = 0; i < this.inputLines.Length; i++)
                    sb.AppendLine($"{i + 1}. \"{this.inputLines[i]}\"");

                String inputLine = this.inputLines[line-1];
                if (charPositionInLine < 0)
                    charPositionInLine = 0;
                if (charPositionInLine > inputLine.Length)
                    charPositionInLine = inputLine.Length;
                sb.Append(inputLine.Substring(0, charPositionInLine));
                sb.Append("-->");
                if (charPositionInLine < inputLine.Length)
                {
                    sb.Append(inputLine[charPositionInLine]);
                    sb.Append("<--");
                    sb.Append(inputLine.Substring(charPositionInLine + 1));
                }
                msgLine = sb.ToString();
            }

            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Error: Error in {this.parserName} at line {line}, column {charPositionInLine}");
                if (msgLine != null)
                    sb.AppendLine(msgLine);
                sb.AppendLine(msg);

                Trace.WriteLine(sb.ToString());
                Console.WriteLine(sb.ToString());
            }
        }
    }
    class LocalErrorListenerLexer : ErrorListener, IAntlrErrorListener<Int32>
    {
        public LocalErrorListenerLexer(String parserName,
            String sourceName,
            String[] inputLines) : base(parserName, sourceName, inputLines)
        {
        }

        public void SyntaxError(TextWriter output,
            IRecognizer recognizer,
            Int32 offendingSymbol,
            Int32 line,
            Int32 charPositionInLine,
            String msg,
            RecognitionException e)
        {
            this.Error(recognizer, line, charPositionInLine, msg, e);
        }
    }


    class LocalErrorListenerParser : ErrorListener, IAntlrErrorListener<IToken>
    {
        public LocalErrorListenerParser(String parserName,
            String sourceName,
            String[] inputLines) : base(parserName, sourceName, inputLines)
        {
        }

        public void SyntaxError(TextWriter output,
        IRecognizer recognizer,
        IToken offendingSymbol,
        Int32 line,
        Int32 charPositionInLine,
        String msg,
        RecognitionException e)
        {
            this.Error(recognizer, line, charPositionInLine, msg, e);
        }
    }
}
