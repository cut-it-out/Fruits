using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource swapAudioSource;
    [SerializeField] AudioSource fruitAudioSource;
    [SerializeField] AudioSource musicAudioSource;

    [SerializeField] AudioClip swapSound;
    [SerializeField] AudioClip fruitSound;

    public bool MusicEnabled { get; private set; }
    public bool SoundEnabled { get; private set; }

    private void Awake()
    {
        MakeSingleton();
        MusicEnabled = true;
        SoundEnabled = true;
    }

    private void MakeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FruitMatch(float volume = 0.8f)
    {
        if (SoundEnabled)
        {
            fruitAudioSource.PlayOneShot(fruitSound, volume);
        }
    }

    public void SwapSound(float volume = 0.4f)
    {
        if (SoundEnabled)
        {
            swapAudioSource.PlayOneShot(swapSound, volume);
        }
    }

    public void SetMusic(bool isEnabled)
    {
        MusicEnabled = isEnabled;
        if (MusicEnabled)
        {
            musicAudioSource.Play();
        }
        else
        {
            musicAudioSource.Stop();
        }
    }

    public void SetSound(bool isEnabled)
    {
        SoundEnabled = isEnabled;
    }
}
