using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestChild
{
    class Program
    {
        static void LaunchRunner(string arg)
        {
            Console.WriteLine("Process0");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "Runner.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = arg;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                Console.WriteLine("Process");
                using (Process exeProcess = Process.Start(startInfo))
                {
                    //Client
                    var client = new NamedPipeClientStream($"PipesOfPiece_{exeProcess.Id}");
                    client.Connect(2000);

                    StreamReader reader = new StreamReader(client);
                    Console.WriteLine("Connected");

                    while (!exeProcess.WaitForExit(1000))
                    {
                        Console.WriteLine(exeProcess.Id);
                        Console.WriteLine("Start readline");
                        var line = reader.ReadToEnd();
                        Console.WriteLine($"Read line={line}");
                    }
                    Console.WriteLine($"Process finished {exeProcess.Id} : {exeProcess.ExitCode}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        static void Start()
        {
            Task[] t = new Task[10];
            for (int i = 0; i < t.Length; i++)
                if (i == 5)
                    t[i] = new Task(() => LaunchRunner("ex"));
                else
                    t[i] = new Task(() => LaunchRunner("test"));
            for (int i = 0; i < t.Length; i++)
            {
                t[i].Start();
                Thread.Sleep(1000);
            }
            Task.WaitAll(t);
            Console.WriteLine("Tasks finished");
        }


        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello world");
                Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
