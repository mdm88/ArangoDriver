using System;
using System.Linq.Expressions;

namespace ArangoDriver.Client.Query.Return
{
    public static class AReturn
    {
        public static AqlReturnFull Variable(string v)
        {
            return new AqlReturnFull(v);
        }

        public static AqlReturnPartial<T> Partial<T>()
        {
            return new AqlReturnPartial<T>();
        }
        
        public static AqlReturnPartial<T> Partial<T>(params Expression<Func<T, object>>[] fields)
        {
            return new AqlReturnPartial<T>(fields);
        }

        public static AqlReturnPartial<T> Partial<T>(params MemberExpression[] fields)
        {
            return new AqlReturnPartial<T>(fields);
        }

        public static AqlReturnManual Manual()
        {
            return new AqlReturnManual();
        }
        
        public static AqlReturnManual Manual(params (string, IAqlReturn)[] fields)
        {
            return new AqlReturnManual(fields);
        }
    }
}