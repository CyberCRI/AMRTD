using UnityEngine;

public class Waypoints : MonoBehaviour
{
    // procedurally filled list of waypoints
    public static Transform[] waypoints;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        waypoints = new Transform[transform.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }
}
