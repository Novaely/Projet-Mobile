using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private string _musicVolumeParam = "MusicVolume";
    [SerializeField] private string _sfxVolumeParam = "SFXVolume";

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioMixerGroup _SFXGroup;
    [SerializeField] private int _sfxPoolSize = 10;

    private List<AudioSource> _sfxPool = new();
    private int _poolIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            GameObject go = new GameObject($"SFX_Source_{i}");
            go.transform.parent = transform;

            AudioSource source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _SFXGroup;
            source.playOnAwake = false;

            _sfxPool.Add(source);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = 0f)
    {
        if (clip == null) return;

        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetNextSource();
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    private AudioSource GetNextSource()
    {
        AudioSource source = _sfxPool[_poolIndex];
        _poolIndex = (_poolIndex + 1) % _sfxPool.Count;
        return source;
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            _mixer.SetFloat(_musicVolumeParam, -80f);
        }
        else
        {
            _mixer.SetFloat(_musicVolumeParam, Mathf.Log10(volume) * 20f);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            _mixer.SetFloat(_sfxVolumeParam, -80f);
        }
        else
        {
            _mixer.SetFloat(_sfxVolumeParam, Mathf.Log10(volume) * 20f);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
