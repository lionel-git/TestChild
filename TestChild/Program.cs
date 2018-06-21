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
                    var server = new NamedPipeServerStream($"PipesOfPiece_{exeProcess.Id}");
                    server.WaitForConnection();
                    StreamReader reader = new StreamReader(server);
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

        static void WaitTask(Task t)
        {
            while (!t.IsCompleted)
                Thread.Sleep(1000);
            Console.WriteLine($"Final status {t.Status}");
        }

        static void Start()
        {
            Task t1 = new Task( () => LaunchRunner("ex"));
            t1.Start();
            Task t2 = new Task( () => LaunchRunner("test"));
            t2.Start();
            Task.WaitAll(t1, t2);
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
