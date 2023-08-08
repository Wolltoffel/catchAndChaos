using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sound system of the game.
/// </summary>
public class SoundSystem : MonoBehaviour
{
    [SerializeField]
    private AudioClip backgroundMusic;

    private static SoundSystem instance;

    private void Start()
    {
        // Set up and play the background music
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = backgroundMusic;
        source.loop = true;
        source.volume = 0.2f;
        source.Play();
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
