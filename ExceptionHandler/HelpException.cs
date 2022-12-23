using System;

namespace NetworkSocket.ExceptionHandler
{
    public class HelpException : Exception
    {
        private const string _message = "Option flags:\n"
                                       + "\t\t-h, --help \t\t\t\t: show help message!\n"
                                       + "\t\t-p, --port \t<port number>\t\t\t: port for server. Defaut 8080\n"
                                       + "\t\t-f, --folder \t<source path>\t\t: source code folder\n";
        public HelpException() : base (_message)
        { 
        }
    }
}
