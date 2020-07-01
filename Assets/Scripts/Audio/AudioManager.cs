//#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : AudioEmitter
{
    public static AudioManager instance = null;

    [SerializeField]
    private float pitchIncrease = 1.05f;

    [SerializeField]
    private AudioMixer mainMixer = null;
    private const string soundGroupKey = "Sound";
    private const string musicGroupKey = "Music";
    private const string mainVolumeKey  = "mainVolume";
    private const string soundvolumeKey = "soundVolume";
    private const string musicvolumeKey = "musicVolume";
    
    [SerializeField]
    private Slider soundSlider;
    [SerializeField]
    private Slider musicSlider;

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
            AudioEmitter.soundMixerGroup = mainMixer.FindMatchingGroups(soundGroupKey)[0];
            AudioEmitter.musicMixerGroup = mainMixer.FindMatchingGroups(musicGroupKey)[0];

            base.Start();
        }
    }

    void Start() {}

    public void onPointerUp(string sliderID)
    {
        float sliderValue = (sliderID == soundGroupKey) ? soundSlider.value : musicSlider.value;
        Debug.Log("POINTER UP! SLIDER " + sliderID + ", value=" + sliderValue);
    }

    public void updateSoundVolume(float input)
    {
        mainMixer.SetFloat(soundvolumeKey, input);

        string mainMixerDebugString = mainMixer == null ? "null" : mainMixer.ToString();
        string soundMixerGroupDebugString = soundMixerGroup == null ? "null" : soundMixerGroup.ToString();
        string musicMixerGroupDebugString = musicMixerGroup == null ? "null" : musicMixerGroup.ToString();
        string outputAudioMixerGroupDebugString = ((mainMixer == null) || (mainMixer.outputAudioMixerGroup == null)) ? "null" : mainMixer.outputAudioMixerGroup.ToString();

        Debug.Log(
            "mainMixer=" + mainMixerDebugString 
        + ", soundMixerGroup=" + soundMixerGroupDebugString
        + ", musicMixerGroup=" + musicMixerGroupDebugString
        + ", outputGroup=" + outputAudioMixerGroupDebugString 
        );
    }

    public void updateMusicVolume(float input)
    {
        mainMixer.SetFloat(musicvolumeKey, input);
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