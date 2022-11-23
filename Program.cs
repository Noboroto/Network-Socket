using System;
using NetworkSocket.Commands;
using NetworkSocket.ExceptionHandler;

namespace NetworkSocket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(args.Length);
                Command c = new Command(args);
                Console.WriteLine(c);
            }
            catch (Exception e)
            {
                ExceptionResponser.Response(e);
            }
        }
    }
}