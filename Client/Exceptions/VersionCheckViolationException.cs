namespace ArangoDriver.Exceptions
{
    public class VersionCheckViolationException : ArangoException
    {
        public string Version { get; }
        
        public VersionCheckViolationException(string version) : base("")
        {
            Version = version;
        }
    }
}