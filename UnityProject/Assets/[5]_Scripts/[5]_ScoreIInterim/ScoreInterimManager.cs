using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BetweenRoundsManager : MonoBehaviour
{
    [SerializeField] Button nextRoundButton;

    void Awake()
    {
        GoToEndScreen();
        if (nextRoundButton== null)
        {
            nextRoundButton = gameObject.AddComponent<Button>();
        }
        nextRoundButton.onClick.AddListener(() => ScreenSwitcher.SwitchScreen(ScreenType.GameScreen));
    }

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

    public void GoToEndScreen()
    {
        if (GameData.GetData<PlayerData>("Child").tempScore>=3 |GameData.GetData<PlayerData>("Parent").tempScore>=3)
        {
            //Activate Endscreen
        }
        
    }
}
