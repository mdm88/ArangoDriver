namespace ArangoDriver.Exceptions
{
    public class UniqueConstraintViolationException : ArangoException
    {
        public UniqueConstraintViolationException() : base("")
        {
        }
    }
}