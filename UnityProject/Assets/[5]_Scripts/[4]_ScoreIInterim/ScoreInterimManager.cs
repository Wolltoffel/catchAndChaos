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
    private ScoreTemplateManager parentScoreManager;
    private ScoreTemplateManager childScoreManager;



    private IEnumerator Start()
    {
        GameEndCircles convergeScript = Instantiate(convergePrefab, CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform).GetComponent<GameEndCircles>();
        convergeScript.ConvergeOn(CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform, CharacterInstantiator.GetActiveCharacter(Characters.Child).transform);

        yield return new WaitForSeconds(1.8f);

        StartCoroutine(ScoreTemplateToOrigin(Characters.Parent));
        yield return StartCoroutine(ScoreTemplateToOrigin(Characters.Child));

        PlayerData parentData = GameData.GetData<PlayerData>("Parent");
        PlayerData childData = GameData.GetData<PlayerData>("Child");
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");

        parentScoreManager.SetScore(parentData.tempScore);
        childScoreManager.SetScore(childData.tempScore);

        ScoreTemplateManager winScore;
        if (data.hasChildWon)
        {
            childData.tempScore++;
            winScore = childScoreManager;
        }
        else
        {
            parentData.tempScore++;
            winScore = parentScoreManager;
        }

        yield return new WaitForSeconds(1);

        parentScoreManager.SetScore(parentData.tempScore);
        childScoreManager.SetScore(childData.tempScore);
        winScore.HighlightWinner();

        yield return new WaitForSeconds(2);

        if (childData.tempScore >= 3 || parentData.tempScore >= 3)
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
        ScoreTemplateManager scoreManager = Instantiate(scoreTemplate).GetComponentInChildren<ScoreTemplateManager>();

        Vector2 currentPos = scoreManager.SetPosition(screenPos);
        Vector2 screenSizes = new(Screen.width, Screen.height);
        Vector2 targetPos = character == Characters.Child ? new(Screen.width * 0.3f, Screen.height / 2) : new(Screen.width * 0.7f,Screen.height / 2);
        scoreManager.SetParent(transform);

        PlayerData data;
        if (character == Characters.Child)
        {
            data = GameData.GetData<ChildData>("Child");
            childScoreManager = scoreManager;
        }
        else
        {
            data = GameData.GetData<ParentData>("Parent");
            parentScoreManager = scoreManager;
        }

        scoreManager.SetName(data.characterAssets.name); //FIX
        scoreManager.SetScore(data.tempScore);

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
