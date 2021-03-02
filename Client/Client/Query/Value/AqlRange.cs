namespace ArangoDriver.Client.Query.Value
{
    public class AqlRange : IAqlValue
    {
        private readonly int _start;
        private readonly int _end;
        
        internal AqlRange(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public string GetExpression(ref int bindCount)
        {
            return $"{_start}..{_end}";
        }

        public object[] GetBindedVars()
        {
            return new object[] {};
        }
    }
}