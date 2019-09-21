using System;
using System.Collections.Generic;

namespace ArangoDriver.Client
{
    // contains ArangoDB specific extension methods, Dictator implementation code can be found in external libraries
    public static partial class DictionaryExtensions
    {
        #region Field _id
        
        /// <summary>
        /// Retrieves value of `_id` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string ID(this Dictionary<string, object> dictionary)
        {
            string id;
            
            try
            {
                id = (string) dictionary["_id"];
                
                if (!Helpers.IsID(id))
                {
                    id = null;
                }
            }
            catch (Exception)
            {
                id = null;
            }
            
            return id;
        }
        
        /// <summary>
        /// Stores `_id` field value.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public static Dictionary<string, object> ID(this Dictionary<string, object> dictionary, string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            dictionary.Add("_id", id);
            
            return dictionary;
        }
        
        #endregion
        
        #region Field _key
        
        /// <summary>
        /// Retrieves value of `_key` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string Key(this Dictionary<string, object> dictionary)
        {
            string key;
            
            try
            {
                key = (string) dictionary["_key"];
                
                if (!Helpers.IsKey(key))
                {
                    key = null;
                }
            }
            catch (Exception)
            {
                key = null;
            }
            
            return key;
        }
        
        /// <summary>
        /// Stores `_key` field value.
        /// </summary>
        /// <exception cref="ArgumentException">Specified key value has invalid format.</exception>
        public static Dictionary<string, object> Key(this Dictionary<string, object> dictionary, string key)
        {
            if (!Helpers.IsKey(key))
            {
                throw new ArgumentException("Specified key value (" + key + ") has invalid format.");
            }
            
            dictionary.Add("_key", key);
            
            return dictionary;
        }
        
        #endregion
        
        #region Field _rev
        
        /// <summary>
        /// Retrieves value of `_rev` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string Rev(this Dictionary<string, object> dictionary)
        {
            string rev;
            
            try
            {
                rev = (string) dictionary["_rev"];
                
                if (!Helpers.IsRev(rev))
                {
                    rev = null;
                }
            }
            catch (Exception)
            {
                rev = null;
            }
            
            return rev;
        }
        
        /// <summary>
        /// Stores `_rev` field value.
        /// </summary>
        /// <exception cref="ArgumentException">Specified rev value has invalid format.</exception>
        public static Dictionary<string, object> Rev(this Dictionary<string, object> dictionary, string rev)
        {
            if (!Helpers.IsRev(rev))
            {
                throw new ArgumentException("Specified rev value (" + rev + ") has invalid format.");
            }   
            
            dictionary.Add("_rev", rev);
            
            return dictionary;
        }

        #endregion

        #region Field _from

        /// <summary>
        /// Retrieves value of `_from` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string From(this Dictionary<string, object> dictionary)
        {
            string from;

            try
            {
                from = (string) dictionary["_from"];

                if (!Helpers.IsID(from))
                {
                    from = null;
                }
            }
            catch (Exception)
            {
                from = null;
            }

            return from;
        }

        /// <summary>
        /// Stores `_from` field value.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public static Dictionary<string, object> From(this Dictionary<string, object> dictionary, string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }

            dictionary.Add("_from", id);

            return dictionary;
        }

        #endregion

        #region Field _to

        /// <summary>
        /// Retrieves value of `_to` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string To(this Dictionary<string, object> dictionary)
        {
            string to;

            try
            {
                to = (string) dictionary["_to"];

                if (!Helpers.IsID(to))
                {
                    to = null;
                }
            }
            catch (Exception)
            {
                to = null;
            }

            return to;
        }

        /// <summary>
        /// Stores `_to` field value.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public static Dictionary<string, object> To(this Dictionary<string, object> dictionary, string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }

            dictionary.Add("_to", id);

            return dictionary;
        }

        #endregion

        /// <summary>
        /// Checks if specified field path has valid document ID value in the format of `collection/key`.
        /// </summary>
        public static bool IsID(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = dictionary[fieldPath];
                
                if (fieldValue is string)
                {
                    return Helpers.IsID((string)fieldValue);
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
    }
}
