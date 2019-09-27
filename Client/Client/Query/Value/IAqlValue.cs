namespace ArangoDriver.Client.Query.Value
{
    public interface IAqlValue
    {
        string GetExpression(ref int bindCount);
        object[] GetBindedVars();
    }
    
    public interface IAqlValue<T> : IAqlValue
    {
        
    }
}