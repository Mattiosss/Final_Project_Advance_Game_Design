using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer sfxMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Audio Clip Libraries")]
    [SerializeField] public Sound[] musicTracks;
    [SerializeField] public Sound[] sfxClips;

    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;

    private List<string> musicPlaylist;
    private int currentMusicIndex = 0;
    private bool playlistMode = false;

    private Coroutine musicFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        musicDictionary = new Dictionary<string, AudioClip>();
        sfxDictionary = new Dictionary<string, AudioClip>();
        musicPlaylist = new List<string>();

        foreach (Sound sound in musicTracks)
        {
            if (!musicDictionary.ContainsKey(sound.name))
            {
                musicDictionary.Add(sound.name, sound.clip);
                musicPlaylist.Add(sound.name);
            }
        }

        foreach (Sound sound in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sound.name))
            {
                sfxDictionary.Add(sound.name, sound.clip);
            }
        }
    }

    private void Start()
    {
        musicMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 0.5f)) * 20f
        );

        sfxMixer.SetFloat(
            "SfxVolume",
            Mathf.Log10(PlayerPrefs.GetFloat("SfxVolume", 0.5f)) * 20f
        );

        PlayMusicPlaylist();
    }

    private void Update()
    {
        if (playlistMode && !musicAudioSource.isPlaying && musicAudioSource.clip != null)
        {
            PlayNextTrack();
        }
    }

    public void PlayMusicForScene(string sceneName)
    {
        if (sceneName == "MenuScene" || sceneName == "LevelSelect")
        {
            PlayMusicPlaylist();
        }
        else
        {
            PlayMusic("GameMusic");
        }
    }

    public void PlayMusicPlaylist(int startIndex = 0)
    {
        if (musicPlaylist.Count == 0)
            return;

        playlistMode = true;
        currentMusicIndex = Mathf.Clamp(startIndex, 0, musicPlaylist.Count - 1);

        PlayMusicInternal(musicPlaylist[currentMusicIndex]);
    }

    private void PlayNextTrack()
    {
        currentMusicIndex++;

        if (currentMusicIndex >= musicPlaylist.Count)
            currentMusicIndex = 0;

        PlayMusicInternal(musicPlaylist[currentMusicIndex]);
    }

    private void PlayMusicInternal(string name)
    {
        if (!musicDictionary.TryGetValue(name, out AudioClip clip))
            return;

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicAudioSource.clip = clip;
        musicAudioSource.loop = false;
        musicAudioSource.volume = 1f;
        musicAudioSource.Play();
    }

    public void PlayMusic(string name)
    {
        playlistMode = false;

        if (!musicDictionary.TryGetValue(name, out AudioClip clip))
        {
            Debug.LogWarning("AudioManager: Music track not found: " + name);
            return;
        }

        if (musicAudioSource.clip == clip && musicAudioSource.isPlaying)
            return;

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicAudioSource.clip = clip;
        musicAudioSource.loop = true;
        musicAudioSource.volume = 1f;
        musicAudioSource.Play();
    }

    public void StopMusic()
    {
        playlistMode = false;
        musicAudioSource.Stop();
    }

    public void FadeOutMusic(float duration)
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(FadeMusicRoutine(0f, duration));
    }

    public void FadeInMusic(float duration)
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(FadeMusicRoutine(1f, duration));
    }

    private IEnumerator FadeMusicRoutine(float targetVolume, float duration)
    {
        float startVolume = musicAudioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        musicAudioSource.volume = targetVolume;
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            sfxAudioSource.PlayOneShot(clip);
            Debug.Log(clip.name);
        }
        else
        {
            Debug.LogWarning("AudioManager: SFX clip not found: " + name);
        }
    }
}