using System.Text;
using NetworkSocket.ExceptionHandler;
using System;
using System.Threading.Tasks;
using System.IO;

namespace NetworkSocket.Commands
{
    public class StartServerCommand
    {
        private const int _defaultPort = 8080;

        private int _port;
        private string _src;

        public int Port => _port;
        public string Src => _src;

        public StartServerCommand(int port, string src)
        {
            _port = port;
            _src = src;
        }

        /// <summary>
        /// Construct Command object from console args
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="InvalidCommandException">Throw when there are wrong flag in command</exception>
        /// <exception cref="HelpException">Throw when help command is called</exception>
        public StartServerCommand(string[] args, Action<int, string>? command = null)
        {
            _src = "./http/";                
            _port = _defaultPort;

            if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
            {
                throw new HelpException();
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-h":
                        case "--help":
                            throw new InvalidCommandException();
                        case "-p":
                        case "-port":
                            if (!int.TryParse(args[i + 1], out _port))
                            {
                                throw new InvalidCommandException("Wrong port number format!");
                            }
                            break;
                        case "-f":
                        case "--folder":
                            if (!Directory.Exists(args[i+1]))
                            {
                                throw new InvalidCommandException("Folder does not exist!");
                            }
                            _src = args[i+1];
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("./");
            stringBuilder.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            stringBuilder.Append(" -p ");
            stringBuilder.Append(_port);
            stringBuilder.Append(" -f ");
            stringBuilder.Append(_src);
            return stringBuilder.ToString();
        }
    }
}
