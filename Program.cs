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
                Console.WriteLine(args.Length);
                StartServerCommand c = new StartServerCommand(args, Server.StartListen);
                Console.WriteLine(c);
                Console.WriteLine("Try go to: http://127.0.0.1:" + c.Port);
                c.Run();
            }
            catch (Exception e)
            {
                ExceptionResponser.Response(e);
            }
        }
    }
}