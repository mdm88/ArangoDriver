namespace ArangoDriver.Client.Query.Update
{
    public interface IAqlUpdate
    {
        string GetExpression(ref int bindCount);
        object[] GetBindedVars();
    }
}