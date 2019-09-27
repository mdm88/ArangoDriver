namespace ArangoDriver.Client.Query.Filter
{
    public interface IAqlFilter
    {
        string GetExpression(ref int bindCount);
        object[] GetBindedVars();
    }
}