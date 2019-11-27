using System.Collections.Generic;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class AResult<T>
    {
        /// <summary>
        /// Determines whether the operation ended with success and returned result value is other than null.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Integer value of the operation response HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Determines if the operation contains value other than null.
        /// </summary>
        public bool HasValue
        {
            get 
            {
                return (Value != null);
            }
        }
        
        /// <summary>
        /// Generic object which type and value depends on performed operation.
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Raw response
        /// </summary>
        public string Raw { get; set; }
        
        /// <summary>
        /// Document which might contain additional information on performed operation.
        /// </summary>
        public Dictionary<string, object> Extra { get; set; }
        
        public AResult() { }
        
        internal AResult(Response response)
        {
            StatusCode = response.StatusCode;
            Raw = response.Body;
        }
    }
}
