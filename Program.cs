using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                Console.WriteLine(args.Length);
                Command c = new Command(args, Server.StartListen);
                Console.WriteLine(c);
                c.Run();
            }
            catch (Exception e)
            {
                ExceptionResponser.Response(e);
            }
        }
    }
}