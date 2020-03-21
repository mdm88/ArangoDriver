using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlSort : IAqlQuery
    {
        private readonly IAqlValue _field;
        private readonly Direction _direction;

        internal AqlSort(IAqlValue field, Direction direction)
        {
            _field = field;
            _direction = direction;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "SORT " + _field.GetExpression(ref bindCount) + (_direction == Direction.Asc ? " ASC" : " DESC");
        }

        public object[] GetBindedVars()
        {
            return _field.GetBindedVars();
        }

        public enum Direction
        {
            Asc,
            Desc
        }
    }
}