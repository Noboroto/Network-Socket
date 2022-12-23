using System;

namespace NetworkSocket.ExceptionHandler
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message) 
        { 
        }
        public InvalidCommandException() : base("Invalid command! Please try again!") 
        {

        }
    }
}
