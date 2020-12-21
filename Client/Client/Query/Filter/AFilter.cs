using System.Collections.Generic;
using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Filter
{
    public static class AFilter
    {
        public static AqlFilter Eq<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter("==", value1, value2);
        }
        public static AqlFilter Neq<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter("!=", value1, value2);
        }
        public static AqlFilter Gt<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter(">", value1, value2);
        }
        public static AqlFilter Gte<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter(">=", value1, value2);
        }
        public static AqlFilter Lt<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter("<", value1, value2);
        }
        public static AqlFilter Lte<T>(IAqlValue<T> value1, IAqlValue<T> value2)
        {
            return new AqlFilter("<=", value1, value2);
        }
        public static AqlFilterAnd Between<T>(IAqlValue<T> value, IAqlValue<T> min, IAqlValue<T> max)
        {
            return And(Gte(value, min), Lte(value, max));
        }

        public static AqlFilter In<T>(IAqlValue<T> value1, AqlArrayValue<T> value2)
        {
            return new AqlFilter("IN", value1, value2);
        }

        public static AqlFilter In<T>(IAqlValue<T> value1, IAqlValue<IEnumerable<T>> value2)
        {
            return new AqlFilter("IN", value1, value2);
        }

        public static AqlFilter Nin<T>(IAqlValue<T> value1, AqlArrayValue<T> value2)
        {
            return new AqlFilter("NOT IN", value1, value2);
        }

        public static AqlFilter Nin<T>(IAqlValue<T> value1, IAqlValue<IEnumerable<T>> value2)
        {
            return new AqlFilter("NOT IN", value1, value2);
        }

        public static AqlFilterOr Or(params IAqlFilter[] filters)
        {
            return new AqlFilterOr(filters);
        }
        public static AqlFilterAnd And(params IAqlFilter[] filters)
        {
            return new AqlFilterAnd(filters);
        }
    }
}