//#define VERBOSEDEBUG
//#define DEVMODE

using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioElement
{
    public AudioEvent audioEvent;
    public string parameter = "";
    public AudioClip clip = null;

    [Range(0f,1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source = null;
}