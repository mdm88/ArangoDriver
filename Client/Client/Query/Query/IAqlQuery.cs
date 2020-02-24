namespace ArangoDriver.Client.Query.Query
{
    public interface IAqlQuery
    {
        string GetExpression(ref int bindCount);
        object[] GetBindedVars();
    }
}