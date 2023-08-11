using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIHandler : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] titleMusic;

    [SerializeField]Button playButton;
    [SerializeField]Button quitButton;

    void Awake()
    {
        playButton.onClick.AddListener(() => ScreenSwitcher.SwitchScreen(Screen.ControllerSelect));
        quitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        SoundSystem.PlayBackgroundMusic(titleMusic);
    }
}

