using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.ExceptionHandler
{
    public static class ExceptionResponser
    {
        public static void Response(Exception e)
        {
            switch (e)
            {
                case HelpException:
                case InvalidCommandException:
                    Console.WriteLine(e.Message);
                    break;
            }
        }
    }
}
