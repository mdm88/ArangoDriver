using System;

namespace ArangoDriver.Exceptions
{
    public class ArangoException : Exception
    {
        public ArangoException(string response) : base(response)
        {
            
        }
    }
}