using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BetweenRoundsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endConditiontText;
    [SerializeField] private Animator conditionAnimator;
    [SerializeField] private GameObject scoreTemplate;

    [SerializeField] private GameObject convergePrefab;
    private ScoreTemplateManager parentScoreManager;
    private ScoreTemplateManager childScoreManager;

    private PlayerData parentData;
    private PlayerData childData;
    private PlayTimeData data;

    private IEnumerator Start()
    {
        //Get Data
        parentData = GameData.GetData<PlayerData>("Parent");
        childData = GameData.GetData<PlayerData>("Child");
        data = GameData.GetData<PlayTimeData>("PlayTimeData");

        GameEndCircles convergeScript = Instantiate(convergePrefab, CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform).GetComponent<GameEndCircles>();
        convergeScript.ConvergeOn(CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform, CharacterInstantiator.GetActiveCharacter(Characters.Child).transform);

        yield return new WaitForSeconds(1.8f);

        StartCoroutine(ScoreTemplateToOrigin(Characters.Parent));
        yield return StartCoroutine(ScoreTemplateToOrigin(Characters.Child));

        endConditiontText.text = GetEndConditionText(data.endCondition);
        conditionAnimator.enabled = true;

        parentScoreManager.SetScore(parentData.tempScore);
        childScoreManager.SetScore(childData.tempScore);
        int newScore = 0;

        ScoreTemplateManager winScore;
        if (data.hasChildWon)
        {
            newScore = ++childData.tempScore;
            winScore = childScoreManager;
        }
        else
        {
            newScore = ++parentData.tempScore;
            winScore = parentScoreManager;
        }

        yield return new WaitForSeconds(1f);

        winScore.RaiseScore(newScore);

        yield return new WaitForSeconds(1.5f);

        if (childData.tempScore >= 3 || parentData.tempScore >= 3)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.EndScreen);
        }
        else
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }
    }

    private string GetEndConditionText(EndCondition endCondition)
    {
        switch (endCondition)
        {
            case EndCondition.Chaos:
                return $"Chaos prevails!";
            case EndCondition.Catch:
                return $"{childData.characterAssets.name} was caught!";
            case EndCondition.Time:
                return $"Time has run out!";
            default:
                return $"If you're seeing this the devs suck!";
        }
    }

    private IEnumerator ScoreTemplateToOrigin(Characters character, float transitionTime = 1)
    {
        Transform transform = CharacterInstantiator.GetActiveCharacter(character).transform;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        ScoreTemplateManager scoreManager = Instantiate(scoreTemplate).GetComponentInChildren<ScoreTemplateManager>();

        Vector2 currentPos = scoreManager.SetPosition(screenPos);
        Vector2 screenSizes = new(Screen.width, Screen.height);
        Vector2 targetPos = character == Characters.Child ? new(Screen.width * 0.3f, Screen.height / 2) : new(Screen.width * 0.7f,Screen.height / 2);
        scoreManager.SetParent(transform);

        PlayerData data;
        if (character == Characters.Child)
        {
            data = childData;
            childScoreManager = scoreManager;
        }
        else
        {
            data = parentData;
            parentScoreManager = scoreManager;
        }

        scoreManager.SetName(data.characterAssets.name);
        scoreManager.SetScore(data.tempScore);
        scoreManager.SetCharacter(character);

        float timeElapsed = 0;
        while (timeElapsed < transitionTime)
        {
            currentPos = Vector2.Lerp(currentPos, targetPos, timeElapsed / transitionTime);
            scoreManager.SetPosition(currentPos);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        scoreManager.SetPosition(targetPos);

    }
}
