#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : AudioEmitter
{
    public static AudioManager instance = null;

    [SerializeField]
    private float pitchIncrease = 1.05f;

    private string backgroundMusicLevelName = "";
    private AudioSource backgroundMusicAudioSource = null;

    protected override void Awake()
    {
        if (null != instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            base.Awake();
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

    public void doFastForward(bool on)
    {
        if (null != backgroundMusicAudioSource)
        {
            backgroundMusicAudioSource.pitch = on ? pitchIncrease : 1f;
        }
    }

}