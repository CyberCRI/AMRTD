//#define VERBOSEDEBUG
//#define DEVMODE

using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioElement
{
    // AudioManager parameters
    public AudioEvent audioEvent;
    public string parameter = "";
    // the AudioSource component that plays the sound
    [HideInInspector]
    public AudioSource source = null;

    // fields to be copied into AudioSource fields
    // the AudioClip resource that is played
    public AudioClip clip = null;
    public bool loop = false;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public string getDebugString()
    {
        string sourceString = (source == null) ? "null" : source.name;
        string sourceIsPlayingString = (source == null) ? "null" : source.isPlaying.ToString();
        string clipString = (clip == null) ? "null" : clip.name;
        return "[AudioElement"
        + " audioEvent: " + audioEvent
        + ", parameter: " + parameter
        + ", source: " + sourceString
        + ", sourceIsPlaying: " + sourceIsPlayingString
        + ", clip: " + clipString
        + ", loop: " + loop
        + ", volume: " + volume
        + ", pitch: " + pitch
        + "]";
    }

}