public class ConnectionData
{
    public System.Guid gameVersionId;

    public ConnectionData(string id)
    {
        gameVersionId = new System.Guid(id);
    }

    public ConnectionData(System.Guid id)
    {
        gameVersionId = new System.Guid(id.ToByteArray());
    }

    public override string ToString()
    {
        return string.Format("[ConnectionData: gameVersionId: {0}]", gameVersionId);
    }
}