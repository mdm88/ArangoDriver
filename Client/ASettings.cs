using System.Text.RegularExpressions;
using fastJSON;

namespace ArangoDriver.Client
{
    public static class ASettings
    {
        internal readonly static Regex KeyRegex = new Regex(@"^[a-zA-Z0-9_\-:.@()+,=;$!*'%]*$");
        
        /// <summary>
        /// Determines driver name.
        /// </summary>
        public const string DriverName = "ArangoDriver";
        
        /// <summary>
        /// Determines driver version.
        /// </summary>
        public const string DriverVersion = "0.1.0";
        
        /// <summary>
        /// Determines JSON serialization options.
        /// </summary>
        public static JSONParameters JsonParameters { get; set; }
        
        static ASettings()
        {
            JsonParameters = new JSONParameters { UseEscapedUnicode = false, UseFastGuid = false, UseExtensions = false };
        }
    }
}