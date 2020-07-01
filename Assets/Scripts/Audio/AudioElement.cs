//#define VERBOSEDEBUG
//#define DEVMODE

using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioElement
{
    // AudioManager parameters
    // the AudioEvent triggered
    public AudioEvent audioEvent;
    public string parameter = "";
    // the AudioSource component of AudioManager that plays the sound
    [HideInInspector]
    public AudioSource source = null;

    // fields to be copied into AudioSource fields
    // the AudioClip resource that is played (wav/mp3)
    public AudioClip clip = null;
    public bool loop = false;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    public bool isMusic = false;

    public AudioElement(
        AudioEvent _audioEvent,
        string _parameter = "",
        AudioSource _source = null,
        AudioClip _clip = null,
        bool _loop = false,
        float _volume = 1f,
        float _pitch = 1f,
        bool _isMusic = false
    )
    {
        audioEvent = _audioEvent;
        parameter = _parameter;
        source = _source;
        clip = _clip;
        loop = _loop;
        volume = _volume;
        pitch = _pitch;
        isMusic = _isMusic;
    }

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
        + ", isMusic: " + isMusic
        + "]";
    }

}