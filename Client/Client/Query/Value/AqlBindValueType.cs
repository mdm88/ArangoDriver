using System;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlBindValueType : AqlBindValue<Type>
    {
        internal AqlBindValueType(Type value) : base(value)
        {
        }

        public override object[] GetBindedVars()
        {
            return new object[] {_value.FullName + ", "  + _value.Assembly.GetName().Name};
        }
    }
}