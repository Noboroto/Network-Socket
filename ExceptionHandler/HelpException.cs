using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.ExceptionHandler
{
    public class HelpException : Exception
    {
        private const string _message = "Options:\n"
                                       + "\t\t-h, --help \t\t\t\t: show help message!\n"
                                       + "\t\t-p, --port \t<port number>\t\t\t: port for server. Defaut 8080\n";
        public HelpException() : base (_message)
        { 
        }
    }
}
