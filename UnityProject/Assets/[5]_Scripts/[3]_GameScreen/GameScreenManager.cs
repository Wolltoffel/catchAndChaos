using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameScreenManager : MonoBehaviour
{
    //Camera
    private CameraManager cameraScript;

    //Parent and Child
    [SerializeField] private GameParent parent;
    [SerializeField] private GameChild child;
    [SerializeField] private GameObject parentObj;
    [SerializeField] private GameObject childObj;

    //Audio
    [SerializeField] 
    private AudioClip[] backgroundAudioClips;

    //Coroutines
    private Coroutine timeCoroutine;
    private static Coroutine checkForChaosUpdate;
    private static GameScreenManager instance;

    //Data
    private ChaosData chaosData;
    private PlayTimeData playTimeData;
    private GameInteractableManager interactableManager;

    //IntroAnimation
    [SerializeField] private GameObject introAnimationPrefab;

    void Awake()
    {
        interactableManager = GetComponent<GameInteractableManager>();

        instance = this;
        Cursor.visible = true;

        SetupGame();
    }

    private void SetupGame()
    {
        SpawnCharacters();

        //Position Camera
        cameraScript = Camera.main.GetComponentInParent<CameraManager>();
        cameraScript.GameCamera();
        cameraScript.TrackPlayers(parentObj.transform, childObj.transform);

        //Set up background Sounds
        if (backgroundAudioClips.Length > 0)
        {
            SoundSystem.PlayBackgroundMusic(backgroundAudioClips);
        }

        //Gets data needed for game
        chaosData = GameData.GetData<ChaosData>("ChaosData");
        playTimeData = GameData.GetData<PlayTimeData>("PlayTimeData");
        chaosData.ResetValues();

        ResetTimeCounter();

        StartCoroutine(_SetupGame());
    }

    private IEnumerator _SetupGame()
    {
        yield return BeforeGameStart();

        interactableManager.LoadInteractablesIntoDatabase();

        StartGame();
    }

    private IEnumerator BeforeGameStart()
    {
        PlayerData child = GameData.GetData<ChildData>("Child");
        PlayerData parent = GameData.GetData<ParentData>("Parent");

        if (child.tempScore == 0 && parent.tempScore == 0)
        {
            //Enable Background Blurr
            var profile = Camera.main.gameObject.GetComponentInChildren<Volume>().profile;
            if (profile.TryGet(out DepthOfField depthOfField))
            {
                depthOfField.active = true;
            }

            yield return new WaitForSeconds(ScreenSwitcher.lastTransitionTime + 0.5f);

            GameObject introAnimation = Instantiate(introAnimationPrefab);
            introAnimation.transform.parent = ScreenSwitcher.currentScreen.transform;
            UIAnimationLengthManager animationLengthManager = introAnimation.GetComponent<UIAnimationLengthManager>();

            yield return new WaitForSeconds(animationLengthManager.animationLength);

            //Disable Background Blurr
            if (profile.TryGet(out depthOfField))
            {
                depthOfField.active = false;
            }
            Destroy(introAnimation);
        }
    }

    private void StartGame()
    {
        //Set up time counter
        SetupTimeCounter();
        InitializeUI();

        parent.enabled = true;
        child.enabled = true;

        checkForChaosUpdate = StartCoroutine(CheckForChaos());
    }

    private void InitializeUI()
    {
        GetComponent<GameUIManager>().enabled = true;
    }

    private void SpawnCharacters()
    {
        CharacterInstantiator.InstantiateCharacter(Characters.Parent, out parentObj, parentObj.transform);
        CharacterInstantiator.InstantiateCharacter(Characters.Child, out childObj, childObj.transform);
    }

    private IEnumerator CheckForChaos()
    {
        while (true)
        {
            if (chaosData.EndGame || playTimeData.hasChildWon)
            {
                EndGame(EndCondition.Chaos);
                checkForChaosUpdate = null;
                yield break;
            }
            yield return null;
        }
    }

    public static void EndGame(EndCondition condition)
    {
        if (checkForChaosUpdate != null)
        {
            instance.StopCoroutine(checkForChaosUpdate);
            checkForChaosUpdate = null;
        }
        
        ScreenSwitcher.OutsourceCoroutine(_EndGame(condition));
    }

    private static IEnumerator _EndGame(EndCondition condition)
    {
        WorldSpaceUI.RemoveAllButtonPrompts();

        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");
        data.hasGameEnded = true;

        switch (condition)
        {
            case EndCondition.Catch | EndCondition.Time:
                break;
            case EndCondition.Chaos:
                data.hasChildWon = true;
                break;
        }

        yield return new WaitForSeconds(1);

        ScreenSwitcher.AddScreen(ScreenType.ScoreInterim);
    }



    #region Timecounter
    private void SetupTimeCounter()
    {
        ResetTimeCounter();
        timeCoroutine = StartCoroutine(TimeCounter());
    }

    private IEnumerator TimeCounter()
    {
        while (playTimeData.RemainingPlayTimeInt > 0)
        {
            yield return new WaitForSeconds(1);
            playTimeData.TempRemainingPlayTime--;
        }

        EndGame(EndCondition.Time);
    }

    private void ResetTimeCounter()
    {
        playTimeData.ResetValues();
    }
    #endregion
}

public enum EndCondition
{
    Chaos,
    Catch,
    Time
}