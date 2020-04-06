//#define VERBOSEDEBUG

using UnityEngine;

public class TrackingEventDataWithIDs : TrackingEventData
{
    // the "player" in the sense of RedMetrics, ie a game session ID
    public string player;
    public string gameVersion;

    public TrackingEventDataWithIDs(
        string _playerGuid,
        string _gameVersionGuid,
        TrackingEvent _trackingEvent,
        CustomData _customData = null
    ) : base(_trackingEvent, _customData)
    {
        player = _playerGuid;
        gameVersion = _gameVersionGuid;

#if VERBOSEDEBUG
    Debug.Log(this.GetType() + " Ctor from strings " + ToString());
#endif
    }

    public TrackingEventDataWithIDs(
        System.Guid _playerGuid,
        System.Guid _gameVersionGuid,
        TrackingEvent _trackingEvent,
        CustomData _customData = null
    ) : base(_trackingEvent, _customData)
    {
        player = _playerGuid.ToString();
        gameVersion = _gameVersionGuid.ToString();

#if VERBOSEDEBUG
    Debug.Log(this.GetType() + " Ctor from System.Guids " + ToString());
#endif
    }

    public TrackingEventDataWithIDs(
        System.Guid _playerGuid,
        System.Guid _gameVersionGuid,
        TrackingEventDataWithoutIDs data
    ) : base(data)
    {
        player = _playerGuid.ToString();
        gameVersion = _gameVersionGuid.ToString();

#if VERBOSEDEBUG
    Debug.Log(this.GetType() + " Ctor from TrackingEventDataWithoutIDs " + ToString());
#endif
    }

    public override string ToString()
    {
        return string.Format("[TrackingEventDataWithIDs player[game session ID]:{0}, gameVersion[game version guid]:{1}, type[tracking event]:{2}, customData:{3}]",
                              player, gameVersion, type, innerCustomData);
    }
}
