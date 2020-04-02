public class TrackingEventDataWithoutIDs : TrackingEventData
{
    public TrackingEventDataWithoutIDs(
        TrackingEvent _trackingEvent,
        CustomData _customData = null,
        string userTime = null
        ) : base(_trackingEvent, _customData, userTime)
    {
    }

    public override string ToString()
    {
        return string.Format("[TrackingEventDataWithoutIDs type:{0} customData:{1}]",
                              type, innerCustomData);
    }
}
