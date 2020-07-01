//#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEmitter : MonoBehaviour
{
    [SerializeField]
    protected AudioElement[] audioElements = null;
    
    public static AudioMixerGroup soundMixerGroup = null;
    public static AudioMixerGroup musicMixerGroup = null;

    protected void Start()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Start");
        #endif

        for (int i = 0; i < audioElements.Length; i++)
        {
            this.audioElements[i].source = this.gameObject.AddComponent<AudioSource>();
            this.audioElements[i].source.clip = audioElements[i].clip;
            this.audioElements[i].source.loop = audioElements[i].loop;
            this.audioElements[i].source.volume = audioElements[i].volume;
            this.audioElements[i].source.pitch = audioElements[i].pitch;
            this.audioElements[i].source.playOnAwake = false;
            this.audioElements[i].source.outputAudioMixerGroup = audioElements[i].isMusic ? AudioEmitter.musicMixerGroup : AudioEmitter.soundMixerGroup ;
        }
    }

    public void stop(AudioEvent audioEvent, string parameter = "")
    {
        #if VERBOSEDEBUG
        Debug.Log("stop(audioEvent=" + audioEvent + ", " + parameter + ")");
        #endif
        play(audioEvent, parameter, false);
    }

    // returns null if the clip is not played due to the dontReplay parameter
    public AudioSource play(AudioEvent audioEvent, string parameter = "", bool doPlay = true, AudioClip dontReplay = null)
    {
        #if VERBOSEDEBUG
        string dontReplayString = (null == dontReplay) ? "null" : dontReplay.name;
        Debug.Log(this.GetType() + " play(" + audioEvent.ToString() + ", " + parameter + ", doPlay=" + doPlay + ", dontReplay=" + dontReplayString + ")");
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

        if ((null != toManage) && (dontReplay != toManage.clip))
        {
            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " play (null != toManage) && (dontReplay != toManage.clip)");
            #endif
            if (doPlay)
            {
                #if VERBOSEDEBUG
                Debug.Log(this.GetType() + " play Play " + toManage.clip.name);
                #endif
                toManage.Play();
            }
            else
            {
                #if VERBOSEDEBUG
                Debug.Log(this.GetType() + " play Stop " + toManage.clip.name + " state isPlaying=" + toManage.isPlaying);
                #endif
                toManage.Stop();
            }
        }
        else
        {
            #if VERBOSEDEBUG
            string toManageString = (null == toManage) ? "null" : toManage.ToString();
            string toManageClipString = ((null == toManage) || (null == toManage.clip)) ? "null" : toManage.clip.name;
            Debug.Log(this.GetType() + " play (toManage == " + toManageString + "), (toManage.clip == " + toManageClipString + ")");
            #endif
            // no modification of the sound, so return null
            toManage = null;
        }

        return toManage;
    }
}