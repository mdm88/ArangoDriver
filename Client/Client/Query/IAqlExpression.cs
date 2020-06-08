namespace ArangoDriver.Client.Query
{
    public interface IAqlExpression
    {
        string GetExpression(ref int bindCount);
        object[] GetBindedVars();
    }

    public interface IAqlExpression<T> : IAqlExpression
    {
        
    }
}