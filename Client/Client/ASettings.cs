using System.Text.RegularExpressions;

namespace ArangoDriver.Client
{
    public static class ASettings
    {
        internal readonly static Regex KeyRegex = new Regex(@"^[a-zA-Z0-9_\-:.@()+,=;$!*'%]*$");
        
        /// <summary>
        /// Determines driver name.
        /// </summary>
        public const string DriverName = "Client";
        
        /// <summary>
        /// Determines driver version.
        /// </summary>
        public const string DriverVersion = "0.1.0";
    }
}