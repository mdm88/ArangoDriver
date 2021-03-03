namespace ArangoDriver.Client.Query.Value
{
    public static class AString
    {
        /// <summary>
        /// SHA1(value)
        /// </summary>
        /// <param name="v">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<string> SHA1(IAqlValue<string> v)
        {
            return new("SHA1", v);
        }
        
        /// <summary>
        /// SHA512(value)
        /// </summary>
        /// <param name="v">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<string> SHA512(IAqlValue<string> v)
        {
            return new("SHA512", v);
        }
        
        /// <summary>
        /// SHA512(value)
        /// </summary>
        /// <param name="v">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<string> JsonStringify<T>(IAqlValue<T> v)
        {
            return new("JSON_STRINGIFY", v);
        }
        
        /// <summary>
        /// SHA512(value)
        /// </summary>
        /// <param name="v">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> JsonParse<T>(IAqlValue<string> v)
        {
            return new("JSON_PARSE", v);
        }
    }
}