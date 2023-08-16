using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BetweenRoundsManager : MonoBehaviour
{
    void Update()
    {
        WaitForKeyInput();
    }

    public void WaitForKeyInput()
    {
        string parentInputDevice = GameData.GetData<PlayerData>("Parent").tempInputDevice;
        string childInputDevice = GameData.GetData<PlayerData>("Child").tempInputDevice;
        if (Input.GetButtonDown(parentInputDevice + "A")|Input.GetButtonDown(childInputDevice + "A"))
            ScreenSwitcher.SwitchScreen (ScreenType.GameScreen);
    }
}
