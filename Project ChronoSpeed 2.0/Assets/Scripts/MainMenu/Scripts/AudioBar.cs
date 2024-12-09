using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioBar : MonoBehaviour
{
    [SerializeField] string MixerName;
    Slider slider;
    OptionsManager optionsManager;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void Start()
    {
        optionsManager = OptionsManager.instance;
        float volume = 0;
        optionsManager.MainMixer.GetFloat(MixerName, out volume);
        slider.value = Mathf.Pow(10, volume / 20);
    }
    public void ValueChanged(float value)
    {
        optionsManager.ChangeMixerVolume(MixerName, value);
    }
}
