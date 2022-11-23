using System.Text;
using NetworkSocket.ExceptionHandler;
using System;
using System.Threading.Tasks;

namespace NetworkSocket.Commands
{
    public class Command
    {
        private const int _defaultPort = 8080;
        private readonly Action<int> _defaultCommand = new Action<int>((port) => { });

        private Action<int> _command;
        private int _port;

        public int Port => _port;

        public Command(int port = _defaultPort, Action<int>? command = null)
        {
            _port = port;
            if (command != null)
            {
                _command = command;
            }
            else
            {
                _command = _defaultCommand;
            }
        }

        /// <summary>
        /// Construct Command object from console args
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="InvalidCommandException">Throw when there are wrong flag in command</exception>
        /// <exception cref="HelpException">Throw when help command is called</exception>
        public Command(string[] args, Action<int>? command = null)
        {
            if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
            {
                throw new HelpException();
            }
            if (args.Length == 0)
            {
                _port = _defaultPort;
            }
            else if (args[0] == "-p" || args[0] == "--port")
            {
                if (!int.TryParse(args[1], out _port))
                {
                    throw new InvalidCommandException();
                }
            }
            else
            {
                throw new InvalidCommandException();
            }
            if (command != null) 
            { 
                _command = command; 
            }
            else 
            { 
                _command = _defaultCommand; 
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("./");
            stringBuilder.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            stringBuilder.Append(" -p ");
            stringBuilder.Append(_port);
            return stringBuilder.ToString();
        }

        public void Run()
        {
            _command(_port);
        }

        public Task RunAsync()
        {
            return Task.Run(() => { _command(_port); }); 
        }
    }
}
