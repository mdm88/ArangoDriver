namespace ArangoDriver.Client
{
    public static class ADocument
    {
        #region Static methods
        
        /// <summary>
        /// Determines if specified value has valid document `_id` format. 
        /// </summary>
        public static bool IsID(string id)
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
        public static bool IsKey(string key)
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
        public static bool IsRev(string revision)
        {
            if (string.IsNullOrEmpty(revision))
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Constructs document ID from specified collection and key values.
        /// </summary>
        public static string Identify(string collection, long key)
        {
            if (string.IsNullOrEmpty(collection))
            {
                return null;
            }
            
            return collection + "/" + key;
        }
        
        /// <summary>
        /// Constructs document ID from specified collection and key values. If key format is invalid null value is returned.
        /// </summary>
        public static string Identify(string collection, string key)
        {
            if (string.IsNullOrEmpty(collection))
            {
                return null;
            }
            
            if (IsKey(key))
            {
                return collection + "/" + key;
            }
            
            return null;
        }
        
        /// <summary>
        /// Parses key value out of specified document ID. If ID has invalid value null is returned. 
        /// </summary>
        public static string ParseKey(string id)
        {
            if (id.Contains("/"))
            {
                var split = id.Split('/');
                
                if ((split.Length == 2) && (split[0].Length > 0) && (split[1].Length > 0))
                {
                    if (IsKey(split[1]))
                    {
                        return split[1];
                    }
                }
            }
            
            return null;
        }
        
        #endregion
    }
}
