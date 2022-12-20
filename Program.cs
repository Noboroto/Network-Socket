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
                Console.WriteLine("Type \"ls\" to get list of clients");
                Console.WriteLine("Type \"active\" to get list of active clients");
                Console.WriteLine("Type \"stop client\" to stop a client\n");
                c.RunAsync();
                while(true)
                {
                    string? val = Console.ReadLine();
                    val = val?.ToLower();
                    switch (val)
                    {
                        case "exit":
                            Console.WriteLine("Press enter to close the program...");
                            Console.ReadLine();
                            return;
                        case "active":
                        case "stop client":
                        case "ls":
                            var list = Server.BrowserClients;
                            if (list == null || list?.Count < 1)
                            {
                                Console.WriteLine("No connection");
                                continue;
                            }
                            for (int i = 0; i < list?.Count; i++)
                            {
                                if (val != "ls" && !list[i].isConnected) continue;
                                Console.Write($"Order {list[i].Position} - {list[i].Address} - ");
                                Console.WriteLine(((!list[i].isConnected) ? "is closed" : "is connecting."));
                            }
                            Console.WriteLine();
                            if (val == "stop client")
                            {
                                Console.Write("Enter client order to stop: ");
                                val= Console.ReadLine();
                                int order = -1;
                                if (!int.TryParse(val, out order))
                                {
                                    Console.WriteLine("Wrong format number");
                                    continue;
                                }
                                if (order < 0 || order >= list?.Count)
                                {
                                    Console.WriteLine("Out of range!");
                                    continue;
                                }
                                if (list?[order].isConnected == false)
                                {
                                    Console.WriteLine("Client has already disconnected!");
                                }
                                list?[order].Close();
                            }
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