using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sound system of the game.
/// </summary>
public class SoundSystem : MonoBehaviour
{
    [Header("Soundeffect Library")]
    [SerializeField]
    private Soundeffects[] soundeffects;
    private static Soundeffects[] staticSoundeffects;

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
        staticSoundeffects = soundeffects;
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
    #endregion
    
    private static AudioClip GetAudioClip(string soundName)
    {
        for (int i = 0; i < staticSoundeffects.Length; i++)
        {
            if (staticSoundeffects[i].Name == soundName)
            {
                var t = staticSoundeffects[i].audioClip;
                if (t == null)
                {
                    throw new System.Exception();
                }

                return staticSoundeffects[i].audioClip;
            }
        }
        throw new System.Exception();
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

    public static void PlaySound(string soundName)
    {
        AudioClip clip = GetAudioClip(soundName);
        PlaySound(clip);
    }

    public static void PlayMusic(string musicName, float length = -1)
    {
        AudioClip clip = GetAudioClip(musicName);
        PlaySound(clip);
    }

    public static void PlayMusic(AudioClip music, float length = -1)
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

[System.Serializable]
class Soundeffects
{
    [SerializeField]
    private string name;
    public string Name { get
        {
            if (name == "" || name.Length == 0)
            {
                return audioClip.name;
            }
            return name;
        }
    }

    public AudioClip audioClip;
}