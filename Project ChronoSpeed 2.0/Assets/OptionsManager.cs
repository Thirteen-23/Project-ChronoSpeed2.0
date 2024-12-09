using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    static OptionsManager instance;
    [SerializeField] AudioMixerGroup SFXVolume;
    [SerializeField] AudioMixerGroup MusicVolume;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
