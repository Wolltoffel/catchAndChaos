using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BetweenRoundsManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI parentText;
    [SerializeField] TextMeshProUGUI childText;
    [SerializeField] GameObject scoreTemplate;

    [SerializeField] private GameObject convergePrefab;

    private IEnumerator Start()
    {
        GameEndCircles convergeScript = Instantiate(convergePrefab, CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform).GetComponent<GameEndCircles>();
        convergeScript.ConvergeOn(CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform, CharacterInstantiator.GetActiveCharacter(Characters.Child).transform);

        yield return new WaitForSeconds(1.8f);

        StartCoroutine(ScoreTemplateToOrigin(Characters.Parent));
        yield return StartCoroutine(ScoreTemplateToOrigin(Characters.Child));

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

    private IEnumerator ScoreTemplateToOrigin(Characters character, float transitionTime = 1)
    {
        Transform transform = CharacterInstantiator.GetActiveCharacter(character).transform;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        ScoreTemplateManager scoreTemplateManager = Instantiate(scoreTemplate).GetComponentInChildren<ScoreTemplateManager>();

        Vector2 currentPos = scoreTemplateManager.SetPosition(screenPos);
        Vector2 screenSizes = new(Screen.width, Screen.height);
        Vector2 targetPos = character == Characters.Child ? new(Screen.width * 0.25f, Screen.height / 2) : new(Screen.width * 0.75f,Screen.height / 2);
        scoreTemplateManager.SetParent(transform);

        PlayerData data;
        if (character == Characters.Child)
        {
            data = GameData.GetData<ChildData>("Child");
        }
        else
        {
            data = GameData.GetData<ParentData>("Parent");
        }

        scoreTemplateManager.SetName(data.characterAssets.name); //FIX
        scoreTemplateManager.SetScore(data.tempScore);

        float timeElapsed = 0;
        while (timeElapsed < transitionTime)
        {
            currentPos = Vector2.Lerp(currentPos, targetPos, timeElapsed / transitionTime);
            scoreTemplateManager.SetPosition(currentPos);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        scoreTemplateManager.SetPosition(targetPos);

    }
}
