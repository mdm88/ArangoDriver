namespace ArangoDriver.Client
{
    internal static class Helpers
    {
        #region Static methods
        
        /// <summary>
        /// Determines if specified value has valid document `_id` format. 
        /// </summary>
        internal static bool IsID(string id)
        {
            if (id.Contains("/"))
            {
                var split = id.Split('/');
                
                if ((split.Length == 2) && (split[0].Length > 0) && (split[1].Length > 0))
                {
                    return IsKey(split[1]);
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Determines if specified value has valid document `_key` format. 
        /// </summary>
        internal static bool IsKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            
            return ASettings.KeyRegex.IsMatch(key);
        }
        
        /// <summary>
        /// Determines if specified value has valid document `_rev` format. 
        /// </summary>
        internal static bool IsRev(string revision)
        {
            if (string.IsNullOrEmpty(revision))
            {
                return false;
            }
            
            return true;
        }
        
        #endregion
    }
}
