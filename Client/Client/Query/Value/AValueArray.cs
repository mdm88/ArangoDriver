using System;
using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public class AValueArray
    {
        /// <summary>
        /// [alias.field1, alias.field2, ...alias.fieldN]
        /// </summary>
        /// <param name="f">Field name including alias</param>
        /// <typeparam name="T">Type of field returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlArrayValue<T> Field<T>(params string[] f)
        {
            return new AqlArrayValue<T>(f.Select(x => new AqlFieldValue<T>(x)));
        }

        /// <summary>
        /// [value1, value2, ...valueN]
        /// </summary>
        /// <param name="v">Array of AqlValue</param>
        /// <typeparam name="T">Type of values</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlArrayValue<T> Value<T>(params IAqlValue<T>[] v)
        {
            return new AqlArrayValue<T>(v);
        }

        /// <summary>
        /// [@var0, @var1, ...@varN] and binds the values
        /// </summary>
        /// <param name="v">Array of values</param>
        /// <typeparam name="T">Type of values</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlArrayValue<T> Bind<T>(params T[] v)
        {
            return new AqlArrayValue<T>(v.Select(x => new AqlBindValue<T>(x)));
        }
        
        /// <summary>
        /// [@var0, @var1, ...@varN] and binds the values
        /// </summary>
        /// <param name="v">Array of values</param>
        /// <returns>AqlValue</returns>
        public static AqlArrayValue<Type> Bind(params Type[] v)
        {
            return new AqlArrayValue<Type>(v.Select(x => new AqlBindValueType(x)));
        }
    }
}