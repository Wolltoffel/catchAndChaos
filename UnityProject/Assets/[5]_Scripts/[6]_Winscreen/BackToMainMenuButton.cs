using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackToMainMenuButton : MonoBehaviour
{
    Button button;

    void Start()
    {
        button.onClick.AddListener(()=>ScreenSwitcher.SwitchScreen (ScreenType.MainMenu));
    }

    void Update()
    {
        WaitForInput();
    }

    void WaitForInput()
    {
         string parentInputDevice = GameData.GetData<PlayerData>("Parent").tempInputDevice;
        string childInputDevice = GameData.GetData<PlayerData>("Child").tempInputDevice;
        if (Input.GetButtonDown(parentInputDevice + "A")|Input.GetButtonDown(childInputDevice + "A"))
            ScreenSwitcher.SwitchScreen (ScreenType.MainMenu);
    }
}
