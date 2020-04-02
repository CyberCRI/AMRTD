/* given up on TypedInfo <- CreatePlayerData
 * TypedInfo <- TrackingEventData
 * caused problems with JsonWriter: JsonException: 'TrackingEventDataWithIDs' already contains the field or alias name 'type'
LitJson.JsonMapper.AddObjectMetadata (System.Type type) (at Assets/UnityLitJson/JsonMapper.cs:242)

public class TypedInfo {
  public string type;

  public TypedInfo() {}

  public override string ToString ()
  {
    return string.Format ("[TypedInfo]");
  }
}
*/

public class CreatePlayerData
{
    public string type = (new TrackingEventDataWithoutIDs(TrackingEvent.CREATEPLAYER)).type;

    public override string ToString()
    {
        return string.Format("[CreatePlayerData: type: {0}]", type);
    }
}