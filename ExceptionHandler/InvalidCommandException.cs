using System;

namespace NetworkSocket.ExceptionHandler
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException() : base("Invalid command! Please try again!") 
        {

        }
    }
}
