using System;

namespace ArangoDriver.Client.Query.Update
{
    public class AqlUpdateSetPartial<T, TV> : IAqlUpdate
    {
        private readonly string _field;
        private readonly string _fieldname;
        private readonly UpdateBuilder<TV> _builder;

        public AqlUpdateSetPartial(string field, string fieldname, Action<UpdateBuilder<TV>> definition)
        {   
            _field = field;
            _fieldname = fieldname;
            _builder = new UpdateBuilder<TV>(fieldname);
            
            definition.Invoke(_builder);
        }
        
        public string GetExpression(ref int bindCount)
        {
            return _field + ":MERGE(" + _fieldname + "," + _builder.GetExpression(ref bindCount) + ")";
        }

        public object[] GetBindedVars()
        {
            return _builder.GetBindedVars();
        }
    }
}