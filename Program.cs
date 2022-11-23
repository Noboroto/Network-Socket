using System;
using NetworkSocket.Commands;

namespace NetworkSocket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            Command c = new Command(args);
            Console.WriteLine(c);
        }
    }
}