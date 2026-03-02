using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetSFXVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (!PlayerPrefs.HasKey("SFXVol"))
        {
            PlayerPrefs.SetFloat("SFXVol", .5f);
        }
    }

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("SFXVol", .5f);
        mixer.SetFloat("SFXVol", Mathf.Log10(slider.value) * 20);
    }

    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat("SFXVol", .5f);
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
}