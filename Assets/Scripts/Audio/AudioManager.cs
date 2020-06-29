#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    [SerializeField]
    private float pitchIncrease = 1.05f;
    [SerializeField]
    private AudioElement[] audioElements = null;

    private string backgroundMusicLevelName = "";
    private AudioSource backgroundMusicAudioSource = null;

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
                audioElements[i].source.loop = audioElements[i].loop;
                audioElements[i].source.volume = audioElements[i].volume;
                audioElements[i].source.pitch = audioElements[i].pitch;
                audioElements[i].source.playOnAwake = false;
            }
        }
    }

    public void stopBackgroundMusic()
    {
        #if VERBOSEDEBUG
        Debug.Log("stopBackgroundMusic");
        #endif
        if (!string.IsNullOrEmpty(backgroundMusicLevelName))
        {
            #if VERBOSEDEBUG
            Debug.Log("stopBackgroundMusic !string.IsNullOrEmpty(backgroundMusicLevelName); backgroundMusicLevelName=" + backgroundMusicLevelName);
            #endif
            stop(AudioEvent.LEVELSTARTS, backgroundMusicLevelName);
            backgroundMusicAudioSource = null;
        }
    }

    public void playBackgroundMusic(string sceneName)
    {
        #if VERBOSEDEBUG
        Debug.Log("playBackgroundMusic(" + sceneName + ")");
        #endif

        if (sceneName != backgroundMusicLevelName)
        {
            #if VERBOSEDEBUG
            Debug.Log("playBackgroundMusic(" + sceneName + ") sceneName != backgroundMusicLevelName");
            #endif
            AudioClip _clip = (backgroundMusicAudioSource == null) ? null : backgroundMusicAudioSource.clip;
            AudioSource result = play(AudioEvent.LEVELSTARTS, sceneName, true, _clip);
            if (null != result) // a new clip is being played, the previous one has to be stopped
            {
                #if VERBOSEDEBUG
                Debug.Log("playBackgroundMusic(" + sceneName + ") sceneName != backgroundMusicLevelName; null != result");
                #endif
                stopBackgroundMusic();
                backgroundMusicAudioSource = result;
                backgroundMusicLevelName = sceneName;
            }
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

    public void doFastForward(bool on)
    {
        if (null != backgroundMusicAudioSource)
        {
            backgroundMusicAudioSource.pitch = on ? pitchIncrease : 1f;
        }
    }

}