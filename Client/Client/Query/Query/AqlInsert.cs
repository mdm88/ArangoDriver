using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArangoDriver.Client.Query.Update;
using ArangoDriver.Client.Query.Value;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlInsert : IAqlQuery
    {
        private readonly string _collectionName;
        private readonly IAqlValue _value;
        private readonly Options _options;

        internal AqlInsert(string collectionName, IAqlValue value, Options options = null)
        {
            _collectionName = collectionName;
            _value = value;
            _options = options;
        }
        
        public string GetExpression(ref int bindCount)
        {
            StringBuilder sb = new StringBuilder("INSERT ");
            sb.Append(_value.GetExpression(ref bindCount));
            sb.Append(" INTO ");
            sb.Append(_collectionName);

            if (_options != null)
            {
                var options = new Dictionary<string, string>();
                if (_options.WaitForSync.HasValue)
                    options.Add(ParameterName.WaitForSync, _options.WaitForSync.Value.ToString().ToLower());
                if (_options.IgnoreErrors.HasValue)
                    options.Add(ParameterName.IgnoreErrors, _options.IgnoreErrors.Value.ToString().ToLower());
                if (_options.Overwrite.HasValue)
                {
                    options.Add(ParameterName.Overwrite, "true");
                    options.Add(ParameterName.OverwriteMode, '"' + _options.Overwrite.Value.ToString().ToLower() + '"');

                    if (_options.Overwrite == OverwriteMode.Update)
                    {
                        if (_options.KeepNull.HasValue)
                            options.Add(ParameterName.KeepNull, _options.KeepNull.Value.ToString().ToLower());
                        if (_options.MergeObjects.HasValue)
                            options.Add(ParameterName.MergeObjects, _options.MergeObjects.Value.ToString().ToLower());
                    }
                }

                sb.Append(" OPTIONS {");
                sb.Append(string.Join(", ", options.Select(kvp => kvp.Key + ":" + kvp.Value)));
                sb.Append('}');
            }
            
            return sb.ToString();
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }

        public class Options
        {
            public bool? IgnoreErrors { get; set; }
            public bool? WaitForSync { get; set; }
            public OverwriteMode? Overwrite { get; set; }
            
            /// <summary>
            /// Only with OverwriteMode.Update
            /// </summary>
            public bool? KeepNull { get; set; }
            
            /// <summary>
            /// Only with OverwriteMode.Update
            /// </summary>
            public bool? MergeObjects { get; set; }
        }
    }
}