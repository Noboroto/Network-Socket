using System;
using System.IO;
using NetworkSocket.ProtocalHandler;

namespace NetworkSocket.ExceptionHandler
{
    public static class ExceptionResponser
    {
        public static Response? Response(Exception e)
        {
            switch (e)
            {
                case FileNotFoundException:
                    Response res = new Response("404.html", 404, "Not Found");
                    return res;
                case HelpException:
                case InvalidCommandException:
                    Console.WriteLine(e.Message);
                    break;
                default: 
                    Console.WriteLine(e);
                    break;
            }
            return null;
        }
    }
}
