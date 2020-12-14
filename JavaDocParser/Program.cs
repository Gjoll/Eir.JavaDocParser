using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eir.JavaDocParser
{
    class Program
    {
        private JavaParser javaParser = new JavaParser();
            void ParseArguments(String[] args)
        {
            Int32 i = 0;
            String GetArg() => args[i++];

            while (i < args.Length)
            {
                String arg = GetArg();
                switch (arg)
                {
                    case "-i":
                        javaParser.InputDirs.Add(GetArg());
                        break;
                    default:
                        throw new NotImplementedException($"Argument {arg} not found");
                }
            }
        }

        void Run()
        {
            this.javaParser.Parse();
        }

        static Int32 Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Program p = new Program();
                p.ParseArguments(args);
                p.Run();
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}
