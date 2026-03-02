using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetMusicVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (!PlayerPrefs.HasKey("MusicVol"))
        {
            PlayerPrefs.SetFloat("MusicVol", .5f);
        }
    }

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("MusicVol", .5f);
        mixer.SetFloat("MusicVol", Mathf.Log10(slider.value) * 20);
    }

    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat("MusicVol", .5f);
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }
}