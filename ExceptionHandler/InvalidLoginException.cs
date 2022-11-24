using System;

namespace NetworkSocket.ExceptionHandler
{
    public class InvalidLoginException : Exception
    {
        public const string ReturnPage = "404.html";
        public InvalidLoginException() : base("Wrong Username or Password! Please try again!") 
        {

        }
    }
}