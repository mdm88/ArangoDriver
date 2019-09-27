using System;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client.Query.Value
{
    public static class AValue
    {
        /// <summary>
        /// alias.field
        /// </summary>
        /// <param name="f">Field name including alias</param>
        /// <typeparam name="T">Type of field returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFieldValue<T> Field<T>(string f)
        {
            return new AqlFieldValue<T>(f);
        }
        
        /// <summary>
        /// alias.field
        /// </summary>
        /// <param name="f">Field name including alias</param>
        /// <returns>AqlValue</returns>
        public static AqlFieldValue<object> Field(string f)
        {
            return new AqlFieldValue<object>(f);
        }

        /// <summary>
        /// DOCUMENT(collectionName, documentKey)
        /// </summary>
        /// <param name="c">Collection Name</param>
        /// <param name="k">AqlValue representing documents key</param>
        /// <typeparam name="T">Type of document returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlCollectionFunctionValue<T> Document<T>(string c, IAqlValue<string> k)
        {
            return new AqlCollectionFunctionValue<T>(c, k);
        }
        
        /// <summary>
        /// ATTRIBUTES(value)
        /// </summary>
        /// <param name="v">AqlFieldValue</param>
        /// <typeparam name="T">Type of document returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFunctionSingleValue<string[]> Attributes<T>(AqlFieldValue<T> v)
        {
            return new AqlFunctionSingleValue<string[]>("ATTRIBUTES", v);
        }

        /// <summary>
        /// value1 op value2
        /// </summary>
        /// <param name="v1">Value</param>
        /// <param name="op">Operand</param>
        /// <param name="v2">Value</param>
        /// <typeparam name="T">Type of values</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlOperationValue<T> Op<T>(IAqlValue<T> v1, string op, IAqlValue<T> v2)
        {
            return new AqlOperationValue<T>(op, v1, v2);
        }

        /// <summary>
        /// @var0 and binds the value
        /// </summary>
        /// <param name="v">Value</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlBindValue<T> Bind<T>(T v)
        {
            return new AqlBindValue<T>(v);
        }
        /// <summary>
        /// @var0 and binds the value
        /// </summary>
        /// <param name="v">Value</param>
        /// <returns>AqlValue</returns>
        public static AqlBindValueType Bind(Type v)
        {
            return new AqlBindValueType(v);
        }
    }
    
    public static class AValue<TO>
    {
        /// <summary>
        /// alias.field
        /// </summary>
        /// <param name="e">Expression representing the field, parameter name is used as alias</param>
        /// <typeparam name="T">Type of field returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFieldValue<T> Field<T>(Expression<Func<TO, T>> e)
        {
            return new AqlFieldValue<T>(new FilterFieldExpression<TO, T>(e).Field);
        }
        
        /// <summary>
        /// alias.field.suffix
        /// </summary>
        /// <param name="e">Expression representing the field, parameter name is used as alias</param>
        /// <param name="suffix">String to add at the end of the field</param>
        /// <typeparam name="T">Type of field returned</typeparam>
        /// <returns>AqlValue</returns>
        public static AqlFieldValue<T> Field<T>(Expression<Func<TO, object>> e, string suffix)
        {
            return new AqlFieldValue<T>(new FilterFieldExpression<TO, object>(e).Field + suffix);
        }
        
        /// <summary>
        /// alias.field.suffix
        /// </summary>
        /// <param name="e">Expression representing the field, parameter name is used as alias</param>
        /// <param name="suffix">String to add at the end of the field</param>
        /// <returns>AqlValue</returns>
        public static AqlFieldValue<object> Field(Expression<Func<TO, object>> e, string suffix)
        {
            return new AqlFieldValue<object>(new FilterFieldExpression<TO, object>(e).Field + suffix);
        }
    }
}