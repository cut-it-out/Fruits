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

    private void Awake()
    {
        MakeSingleton();
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
        fruitAudioSource.PlayOneShot(fruitSound, volume);
    }

    public void SwapSound(float volume = 0.4f)
    {
        swapAudioSource.PlayOneShot(swapSound, volume);
    }

}
