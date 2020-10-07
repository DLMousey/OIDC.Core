using System;

namespace OAuthServer.Exceptions
{
    public class UnknownApplicationException : Exception
    {
        public UnknownApplicationException(string message) : base(message)
        {
        }
    }
}