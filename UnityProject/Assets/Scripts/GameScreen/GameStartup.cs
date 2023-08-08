using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    [SerializeField] 
    private AudioClip[] backgroundAudioClips;

    void Start()
    {
        gameObject.GetComponent<GameUIManager>();
        
        gameObject.AddComponent<GameChild>();
        gameObject.AddComponent<GameParent>();

        if (backgroundAudioClips.Length > 0)
        {
            SoundSystem.PlayBackgroundMusic(backgroundAudioClips);
        }
    }
}
