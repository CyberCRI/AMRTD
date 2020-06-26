#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioElement[] audioElements = null;

    void Awake()
    {
        if (null != instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;

            for (int i = 0; i < audioElements.Length; i++)
            {
                audioElements[i].source = this.gameObject.AddComponent<AudioSource>();
                audioElements[i].source.clip = audioElements[i].clip;
                audioElements[i].source.volume = audioElements[i].volume;
                audioElements[i].source.pitch = audioElements[i].pitch;
                audioElements[i].source.playOnAwake = false;
            }
        }
    }

    public void stop(AudioEvent audioEvent, string parameter = "")
    {
        play(audioEvent, parameter, false);
    }

    public void play(AudioEvent audioEvent, string parameter = "", bool doPlay = true)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " play(" + audioEvent.ToString() + ", " + parameter + ")");
        #endif

        AudioElement[] matches = Array.FindAll<AudioElement>(audioElements, audioElement => audioElement.audioEvent == audioEvent);
        
        #if VERBOSEDEBUG
        string counts = (null != matches) ? matches.Length.ToString() : "0";
        Debug.Log(this.GetType() + " play(" + audioEvent.ToString() + ", " + parameter + ") found " + counts + " matches");
        #endif

        AudioSource toManage = null;
        
        if (null != matches)
        {
            if (matches.Length == 1)
            {
                toManage = matches[0].source;
            }
            else
            {
                AudioElement[] submatches = Array.FindAll<AudioElement>(matches, audioElement => audioElement.parameter == parameter);
        
                #if VERBOSEDEBUG
                string subcounts = (null != submatches) ? submatches.Length.ToString() : "0";
                Debug.Log(this.GetType() + " play(" + audioEvent.ToString() + ", " + parameter + ") found " + subcounts + " submatches");
                #endif

                if ((null != submatches) && (submatches.Length == 1))
                {
                    toManage = submatches[0].source;
                }
            }
        }

        if (null != toManage)
        {
            if (doPlay)
            {
                toManage.Play();
            }
            else
            {
                toManage.Stop();
            }
        }
    }

}