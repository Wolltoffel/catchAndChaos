using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIHandler : MonoBehaviour
{
    [SerializeField]Button playButton;
    [SerializeField]Button quitButton;

    void Awake()
    {
        playButton.onClick.AddListener(()=>ScreenSwitcher.SwitchScreen(Screen.ControllerSelect));
        quitButton.onClick.AddListener (()=> Application.Quit());
    }

}

