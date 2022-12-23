using System;
using System.Collections.Generic;
using System.IO;

using NetworkSocket.Commands;
using NetworkSocket.ExceptionHandler;
using NetworkSocket.ProtocalHandler;
using NetworkSocket.SocketHandler;
using NetworkSocket.User;

namespace NetworkSocket
{
    internal class Program
    {
        static List<string> CommandLines = new List<string>
        {
            "Type \"help\" to print this message",
            "Type \"exit\" to stop the server.",
            "Type \"restart\" to restart the server",
            "Type \"ls\" to get list of clients",
            "Type \"active\" to get list of active clients",
            "Type \"path\" to change the source code folder",
            "Type \"port\" to change restart server and change to another port",
            "Type \"register\" to create new account",
            "Type \"stop client\" to stop a client\n",
        };

        static void Main(string[] args)
        {
            try
            {
                StartServerCommand command = new StartServerCommand(args);
                Server s;
                
            Start:
                s = new Server(command.Port, command.Src);
                s.StartListenAsync();

                Console.Write("Command: \"");
                Console.WriteLine(command + "\"\n");
                Console.WriteLine("Try go to: http://127.0.0.1:" + command.Port);
                foreach (var line in CommandLines)
                {
                    Console.WriteLine(line);
                }

                while (true)
                {
                    string? val = Console.ReadLine();
                    val = val?.ToLower()
                              .TrimEnd()
                              .TrimStart();
                    switch (val)
                    {
                        case "help":
                            Console.WriteLine($"Command: \"{command}\"\n");
                            Console.WriteLine("Try go to: http://127.0.0.1:" + command.Port);
                            foreach (var line in CommandLines)
                            {
                                Console.WriteLine(line);
                            }
                            break;
                        case "register":
                            Console.Write("Username: ");
                            var username = Console.ReadLine();
                            Console.Write("Password: ");
                            var password = Console.ReadLine();

                            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
                                break;
                            
                            UserInfo info = new UserInfo(username, password);
                            s.RegisterUser(info);

                            Console.WriteLine("Add successful");
                            break;
                        case "restart":
                            s.Close();
                            goto Start;
                        case "port":
                            Console.Write("Enter new port: ");
                            int port = 8080;
                            int.TryParse(Console.ReadLine(), out port);

                            if (port == command.Port)
                            {
                                Console.WriteLine($"Server already run on port {port}");
                                continue;
                            }

                            command = new StartServerCommand(port, command.Src);
                            s.Close();
                            goto Start;
                        case "path":
                            Console.Write("Enter new source code path: ");
                            var input = Console.ReadLine();
                            if (!string.IsNullOrEmpty(input))
                            {
                                s.SrcPath = input;
                                command = new StartServerCommand(command.Port, input);
                                Console.WriteLine($"New command: \"{command}\"\n");
                                Console.WriteLine("Try go to: http://127.0.0.1:" + command.Port);
                            }
                            break;
                        case "active":
                        case "stop client":
                        case "ls":
                            var list = s.BrowserClients;
                            if (list == null || list?.Count < 1)
                            {
                                Console.WriteLine("No connection");
                                continue;
                            }
                            for (int i = 0; i < list?.Count; i++)
                            {
                                if (val != "ls" && !list[i].isConnected) continue;
                                Console.Write($"Order {list[i].Position} - {list[i].Address} - requested {list[i].ResquestCounter} time" + ((list[i].ResquestCounter > 1) ? "s" : ""));
                                Console.WriteLine(((!list[i].isConnected) ? "is closed" : ""));
                            }
                            Console.WriteLine();
                            if (val == "stop client")
                            {
                                Console.Write("Enter client order to stop: ");
                                val = Console.ReadLine();
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
                        case "exit":
                            Console.WriteLine("Press enter to close the program...");
                            Console.ReadLine();
                            return;
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