using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    static public OptionsManager instance;
    public AudioMixer MainMixer;
    public bool IsControllerXbox;
    public bool IsControlsLoading;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }       
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mixer"> Master, Music, SFX</param>
    /// <param name="volume"></param>
    public void ChangeMixerVolume(string mixer, float volume)
    {
        if (mixer == "Master" || mixer == "Music" || mixer == "SFX")
            MainMixer.SetFloat(mixer, Mathf.Log10(volume) * 20);
        else
            Debug.Log(mixer + "- is not a proper mixer name");
    }

    public void SetIsControlsLoading(bool IsIt)
    {
        IsControlsLoading = IsIt;
    }
}
