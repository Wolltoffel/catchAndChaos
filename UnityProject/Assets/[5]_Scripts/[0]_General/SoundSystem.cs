using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Manages the sound system of the game.
/// </summary>
public class SoundSystem : MonoBehaviour
{
    [Header("Background Music Stuff")]
    private static AudioSource backgroundMusicPlayer;
    private static Coroutine backgroundMusicCoroutine;
    private static Coroutine overrideMusicCoroutine;

    private static SoundSystem instance;

    #region Startup
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

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion

    private static AudioClip GetAudioClip(string soundName)
    {
        return Resources.Load<AudioClip>(soundName);
    }

    #region PlaySound
    /// <summary>
    /// Plays the specified sound.
    /// </summary>
    /// <param name="soundName">The name of the audioclip to play.</param>
    public static void PlaySound(string soundName)
    {
        AudioClip clip = GetAudioClip(soundName);
        PlaySound(clip);
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
    #endregion

    #region PlayMusic
    /// <summary>
    /// Plays the specified music and silences the background music.
    /// </summary>
    /// <param name="musicName">The name of the audioclip to play.</param>
    /// <param name="length">For how long the music should play.</param>
    public static void PlayMusic(string musicName, float length = -1)
    {
        AudioClip clip = GetAudioClip(musicName);
        PlayMusic(clip, length);
    }

    public static void PlayMusic(AudioClip music, float length)
    {
        if (overrideMusicCoroutine != null)
            instance.StopCoroutine(overrideMusicCoroutine);
        IEnumerator i = instance._PlayMusic(music, length);
        overrideMusicCoroutine = instance.StartCoroutine(i);
    }

    private IEnumerator _PlayMusic(AudioClip clip, float length)
    {
        float backgroundMusicVolume = backgroundMusicPlayer.volume;
        float timeElapsed = 0;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        length = length <= 0 ? clip.length : length;

        backgroundMusicPlayer.volume = 0;
        source.volume = backgroundMusicVolume;
        source.clip = clip;
        source.Play();

        while (timeElapsed < length)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Object.Destroy(source);
        backgroundMusicPlayer.volume = backgroundMusicVolume;

        overrideMusicCoroutine = null;
        yield break;
    }
    #endregion

    #region Background Music
    public static void PlayBackgroundMusic(AudioClip[] musicClips)
    {
        if (musicClips.Length > 0)
        {
            if (backgroundMusicCoroutine != null)
                instance.StopCoroutine(backgroundMusicCoroutine);
            IEnumerator i = instance._PlayBackgroundMusic(musicClips);
            instance.StartCoroutine(i);
        }
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
    #endregion
}