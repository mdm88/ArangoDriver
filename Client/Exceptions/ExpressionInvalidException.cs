namespace ArangoDriver.Exceptions
{
    public class ExpressionInvalidException : ArangoException
    {
        public ExpressionInvalidException(string response) : base(response)
        {
        }
    }
}