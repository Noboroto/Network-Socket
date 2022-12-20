using System;

using NetworkSocket.Commands;
using NetworkSocket.ExceptionHandler;
using NetworkSocket.SocketHandler;

namespace NetworkSocket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StartServerCommand c = new StartServerCommand(args, Server.StartListen);
                Console.Write("Command: \"");
                Console.WriteLine(c + "\"\n");
                Console.WriteLine("Try go to: http://127.0.0.1:" + c.Port);
                Console.WriteLine("Type \"exit\" to stop the server.");
                Console.WriteLine("Type \"ls\" to get list of client\n");
                c.RunAsync();
                while(true)
                {
                    string? val = Console.ReadLine();
                    val = val?.ToLower();
                    switch (val)
                    {
                        case "exit":
                            Console.WriteLine("Program stop.");
                            return;
                        case "ls":
                            var list = Server.BrowserClients;
                            if (list == null || list?.Count < 1)
                            {
                                Console.WriteLine("No connection");
                                continue;
                            }
                            for (int i = 0; i < list?.Count; i++)
                            {
                                Console.Write($"Order {list[i].Position} - {list[i].Address} - ");
                                Console.WriteLine(((!list[i].isConnected) ? "is closed" : "is connecting."));
                            }
                            Console.WriteLine();
                            continue;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionResponser.Response(e);
            }
        }
    }
}