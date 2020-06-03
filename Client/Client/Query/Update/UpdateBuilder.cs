using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Client.Query.Value;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client.Query.Update
{
    public class UpdateBuilder<T>
    {
        private readonly List<IAqlUpdate> _updates;
        private readonly string _fieldname;

        public UpdateBuilder()
        {
            _updates = new List<IAqlUpdate>();
        }

        public UpdateBuilder(string fieldname) : this()
        {
            _fieldname = fieldname;
        }
        
        public UpdateBuilder<T> Set<TV>(Expression<Func<T, TV>> field, IAqlValue<TV> value)
        {
            var expression = new FieldExpression<T, TV>(field);

            _updates.Add(new AqlUpdateSetValue(expression.Field, value));

            return this;
        }
        
        public UpdateBuilder<T> SetPartial<TV>(Expression<Func<T, TV>> field, Action<UpdateBuilder<TV>> build)
        {
            var expression = new FieldExpression<T, TV>(field);

            _updates.Add(new AqlUpdateSetPartial<T, TV>(expression.Field, (_fieldname ?? expression.Name) + "." + expression.Field, build));

            return this;
        }
        
        public UpdateBuilder<T> Inc<TV>(Expression<Func<T, TV>> field, IAqlValue<TV> build)
        {
            var expression = new FieldExpression<T, TV>(field);

            _updates.Add(new AqlUpdateInc(expression.Field, (_fieldname ?? expression.Name), build));

            return this;
        }
        
        public string GetExpression(ref int bindCount)
        {
            string expression = "";
            foreach (IAqlUpdate update in _updates)
            {
                expression += update.GetExpression(ref bindCount) + ", ";
            }
            expression = expression.Substring(0, expression.Length-2);
            
            return "{ " + expression + " }";
        }

        public object[] GetBindedVars()
        {
            return _updates.SelectMany(x => x.GetBindedVars()).ToArray();
        }

        public bool IsEmpty()
        {
            return !_updates.Any();
        }
    }
}