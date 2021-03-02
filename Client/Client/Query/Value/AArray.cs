using System.Collections;

namespace ArangoDriver.Client.Query.Value
{
    public static class AArray
    {   
        /// <summary>
        /// MINUS(value,...value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Minus<T>(params IAqlValue<T>[] v) where T : IEnumerable
        {
            return new AqlFunctionValue<T>("MINUS", v);
        }
        
        /// <summary>
        /// UNION(value,...value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<T> Union<T>(params IAqlValue<T>[] v) where T : IEnumerable
        {
            return new AqlFunctionValue<T>("UNION", v);
        }
        
        /// <summary>
        /// COUNT(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of value returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<int> Count<T>(IAqlValue<T> v) where T : IEnumerable
        {
            return new AqlFunctionValue<int>("COUNT", v);
        }

        /// <summary>
        /// RANGE(start, end, step)
        /// </summary>
        /// <param name="start">AqlValue</param>
        /// <param name="end">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<int> Range(IAqlValue<int> start, IAqlValue<int> end)
        {
            return new AqlFunctionValue<int>("RANGE", start, end);
        }

        /// <summary>
        /// RANGE(start, end, step)
        /// </summary>
        /// <param name="start">AqlValue</param>
        /// <param name="end">AqlValue</param>
        /// <param name="step">AqlValue</param>
        /// <returns>AqlValue</returns>
        public static AqlFunctionValue<int> Range(IAqlValue<int> start, IAqlValue<int> end, IAqlValue<int> step)
        {
            return new AqlFunctionValue<int>("RANGE", start, end, step);
        }
    }
}