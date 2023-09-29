using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BetweenRoundsManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI parentText;
    [SerializeField] TextMeshProUGUI childText;

    [SerializeField] private GameObject convergePrefab;

    IEnumerator Start()
    {
        GameEndConverge convergeScript = Instantiate(convergePrefab, CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform).GetComponent<GameEndConverge>();
        convergeScript.ConvergeOn(CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform, CharacterInstantiator.GetActiveCharacter(Characters.Child).transform);

        PlayerData parent = GameData.GetData<PlayerData>("Parent");
        PlayerData child = GameData.GetData<PlayerData>("Child");
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");

        parentText.text = parent.tempScore.ToString();
        childText.text = child.tempScore.ToString();

        if (data.hasChildWon)
            child.tempScore++;
        else
            parent.tempScore++;

        yield return new WaitForSeconds(1);

        parentText.text = parent.tempScore.ToString();
        childText.text = child.tempScore.ToString();

        yield return new WaitForSeconds(2);

        if (child.tempScore >= 3 || parent.tempScore >= 3)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.EndScreen);
        }
        else
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }
    }
}
