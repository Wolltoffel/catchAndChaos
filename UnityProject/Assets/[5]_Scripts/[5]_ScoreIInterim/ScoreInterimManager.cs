using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BetweenRoundsManager : MonoBehaviour
{
    [SerializeField] Button nextRoundButton;
    [SerializeField] TextMeshPro parentText;
    [SerializeField] TextMeshPro childText;

    IEnumerator Start()
    {
        PlayerData parent = GameData.GetData<PlayerData>("Parent");
        PlayerData child = GameData.GetData<PlayerData>("Child");
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");


        if (nextRoundButton== null)
        {
            nextRoundButton = gameObject.AddComponent<Button>();
        }
        nextRoundButton.onClick.AddListener(() => ScreenSwitcher.SwitchScreen(ScreenType.GameScreen));

        parentText.text = parent.tempScore.ToString();
        childText.text = child.tempScore.ToString();

        if (data.hasChildWon)
            child.tempScore++;
        else
            parent.tempScore++;

        yield return new WaitForSeconds(1);

        parentText.text = parent.tempScore.ToString();
        childText.text = child.tempScore.ToString();

        yield return new WaitForSeconds(1);

        if (child.tempScore >= 3 || parent.tempScore >= 3)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.EndScreen);
        }
        else
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }
    }

    /*void Update()
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
        
    }*/
}
