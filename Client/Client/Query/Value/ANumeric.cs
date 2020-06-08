using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public static class ANumeric
    {
        private static AqlFunctionValue<T> Basic<T>(string f, params IAqlValue<T>[] v)
        {
            IAqlValue<T> value;
            if (v.Length == 1)
                value = v.First();
            else
                value = new AqlArrayValue<T>(v);

            return new AqlFunctionValue<T>(f, value);
        }
        
        private static AqlFunctionValue<T> Multiple<T>(string f, params IAqlValue<T>[] v)
        {
            return new AqlFunctionValue<T>(f, v);
        }
        
        /// <summary>
        /// MIN(value) or MIN([value,...value])
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Min<T>(params IAqlValue<T>[] v)
        {
            return Basic("MIN", v);
        }
        
        /// <summary>
        /// MAX(value) or MAX([value,...value])
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Max<T>(params IAqlValue<T>[] v)
        {
            return Basic("MAX", v);
        }
        
        /// <summary>
        /// AVG(value) or AVG([value,...value])
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Avg<T>(params IAqlValue<T>[] v)
        {
            return Basic("AVG", v);
        }
        
        /// <summary>
        /// SUM(value) or SUM([value,...value])
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Sum<T>(params IAqlValue<T>[] v)
        {
            return Basic("SUM", v);
        }
        
        /// <summary>
        /// POW(value, exp)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <param name="e">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Pow<T>(IAqlValue<T> v, IAqlValue<T> e)
        {
            return Multiple("POW", v, e);
        }
        
        /// <summary>
        /// SQRT(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Sqrt<T>(IAqlValue<T> v)
        {
            return Basic("SQRT", v);
        }
        
        /// <summary>
        /// ABS(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Abs<T>(IAqlValue<T> v)
        {
            return Basic("ABS", v);
        }
        
        /// <summary>
        /// CEIL(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Ceil<T>(IAqlValue<T> v)
        {
            return Basic("CEIL", v);
        }
        
        /// <summary>
        /// FLOOR(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Floor<T>(IAqlValue<T> v)
        {
            return Basic("FLOOR", v);
        }
        
        /// <summary>
        /// ROUND(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Round<T>(IAqlValue<T> v)
        {
            return Basic("ROUND", v);
        }

        /// <summary>
        /// IF_NULL(value1, value2, ..., valueN)
        /// </summary>
        /// <param name="v">AqlValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Coalesce<T>(params IAqlValue<T>[] v)
        {
            return Multiple("NOT_NULL", v);
        }
    }
}