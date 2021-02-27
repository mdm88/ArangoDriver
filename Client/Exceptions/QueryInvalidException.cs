namespace ArangoDriver.Exceptions
{
    public class QueryInvalidException : ArangoException
    {
        public QueryInvalidException()
        {
        }
        
        public QueryInvalidException(string response) : base(response)
        {
        }
    }
}