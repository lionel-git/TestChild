using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Runner
{
    class Program
    {
        static int Main(string[] args)
        {
            var server = new NamedPipeServerStream($"PipesOfPiece_{ Process.GetCurrentProcess().Id}");
            server.WaitForConnection();
            StreamWriter writer = new StreamWriter(server);

            try
            {
                Console.WriteLine("Start Runner");





                foreach (var arg in args)
                {
                    Console.WriteLine(arg);
                }
                if (args.Length >= 1 && args[0] == "ex")
                {
                    Thread.Sleep(2000);
                    throw new Exception("error");
                }            // Do some calc
                Thread.Sleep(10000);
                Console.WriteLine("Calc finished");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                writer.WriteLine(e.Message);
                writer.WriteLine("Suite1");
                writer.WriteLine("Suite2");
                writer.Flush();
                return 1;
            }

            writer.WriteLine("OK");
            writer.Flush();
            return 0;
        }
    }
}
