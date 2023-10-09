using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameButtons : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button replayButton;

    void Start()
    {
        mainMenuButton.onClick.AddListener(()=>ScreenSwitcher.SwitchScreen (ScreenType.MainMenu));
        replayButton.onClick.AddListener(()=>ScreenSwitcher.SwitchScreen (ScreenType.GameScreen));
    }

    void Update()
    {
        WaitForInputHome();
        WaitForInputReplay();
    }

    void WaitForInputHome()
    {
         string parentInputDevice = GameData.GetData<PlayerData>("Parent").tempInputDevice;
        string childInputDevice = GameData.GetData<PlayerData>("Child").tempInputDevice;
        if (Input.GetButtonDown(parentInputDevice + "B")|Input.GetButtonDown(childInputDevice + "B"))
            ScreenSwitcher.SwitchScreen (ScreenType.MainMenu);
    }

    void WaitForInputReplay()
    {
        string parentInputDevice = GameData.GetData<PlayerData>("Parent").tempInputDevice;
        string childInputDevice = GameData.GetData<PlayerData>("Child").tempInputDevice;
        if (Input.GetButtonDown(parentInputDevice + "A") | Input.GetButtonDown(childInputDevice + "A"))
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
    }
}
