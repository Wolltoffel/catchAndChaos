using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sound system of the game.
/// </summary>
public class SoundSystem : MonoBehaviour
{
    [Header("Background Music Stuff")]
    private static AudioSource backgroundMusicPlayer;
    private static Coroutine backgroundMusicCoroutine;

    private static SoundSystem instance;

    private void Awake()
    {
        // Set up and play the background music
        backgroundMusicPlayer = gameObject.AddComponent<AudioSource>();
        backgroundMusicPlayer.loop = true;
        backgroundMusicPlayer.volume = 0.2f;
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// Plays the specified sound.
    /// </summary>
    /// <param name="soundType">The type of sound to play.</param>
    public static void PlaySound(AudioClip soundClip)
    {
        IEnumerator i = instance._PlaySound(soundClip);
        instance.StartCoroutine(i);
    }

    public static void PlayBackgroundMusic(AudioClip[] musicClips)
    {
        if (backgroundMusicCoroutine != null)
            instance.StopCoroutine(backgroundMusicCoroutine);
        IEnumerator i = instance._PlayBackgroundMusic(musicClips);
        instance.StartCoroutine(i);
    }

    private IEnumerator _PlayBackgroundMusic(AudioClip[] musicClips, float fadeTime = 3)
    {
        AudioClip[] backgroundMusicDatabase = musicClips;
        int backgroundMusicPointer = Random.Range(0,backgroundMusicDatabase.Length);

        while (true)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            float targetVolume = backgroundMusicPlayer.volume;
            float currentVolume = 0;
            source.volume = currentVolume;

            source.clip = backgroundMusicDatabase[backgroundMusicPointer];
            source.Play();

            while (currentVolume < targetVolume)
            {
                source.volume = currentVolume;
                backgroundMusicPlayer.volume = targetVolume - currentVolume;

                currentVolume += targetVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            source.volume = targetVolume;

            Object.Destroy(backgroundMusicPlayer);
            backgroundMusicPlayer = source;

            yield return new WaitForSeconds(backgroundMusicDatabase[backgroundMusicPointer].length - fadeTime);

            backgroundMusicPointer = (backgroundMusicPointer + 1) % backgroundMusicDatabase.Length;
        }
    }


    private IEnumerator _PlaySound(AudioClip audioClip)
    {
        // Create a new AudioSource component to play the sound
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.Play();

        // Wait for the sound to finish playing
        yield return new WaitForSeconds(audioClip.length);

        // Destroy the AudioSource component to clean up
        Component.Destroy(source);
    }
}
