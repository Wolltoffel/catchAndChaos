using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenManager : MonoBehaviour
{
    [SerializeField] private CameraManager camera;

    //Parent and Child
    [SerializeField] private GameParent parent;
    [SerializeField] private GameChild child;
    [SerializeField] private GameObject parentObj;
    [SerializeField] private GameObject childObj;

    //Audio
    [SerializeField] 
    private AudioClip[] backgroundAudioClips;

    //TimeCounter
    private float timeRemaining;
    private Coroutine timeCoroutine;

    //Data
    private ChaosData chaosData;
    private PlayTimeData playTimeData;
    private GameInteractableManager interactableManager;

    void Awake()
    {
        interactableManager = GetComponent<GameInteractableManager>();
        SetupGame();
    }

    private void SetupGame()
    {
        SpawnCharacters();
        interactableManager.LoadInteractablesIntoDatabase();

        //Position Camera
        camera = Camera.main.GetComponent<CameraManager>();
        camera.GameCamera();
        camera.TrackPlayers(parentObj.transform,childObj.transform);

        //Set up background Sounds
        if (backgroundAudioClips.Length > 0)
        {
            SoundSystem.PlayBackgroundMusic(backgroundAudioClips);
        }

        //Gets data needed for game
        chaosData = GameData.GetData<ChaosData>("ChaosData");
        playTimeData = GameData.GetData<PlayTimeData>("PlayTimeData");
        playTimeData.ResetValues();
        chaosData.ResetValues();

        //Set up time counter
        SetupTimeCounter();
    }

    private void SpawnCharacters()
    {
        CharacterInstantiator.InstantiateCharacter(Characters.Parent, out parentObj, parentObj.transform);
        CharacterInstantiator.InstantiateCharacter(Characters.Child, out childObj, childObj.transform);
    }

    private void Update()
    {
        if (chaosData.EndGame)
        {
            EndGame(EndCondition.Chaos);
        }
    }

    public static void EndGame(EndCondition condition)
    {
        ScreenSwitcher.OutsourceCoroutine(_EndGame(condition));
    }

    private static IEnumerator _EndGame(EndCondition condition)
    {
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");
        data.hasGameEnded = true;


        PlayerData child = GameData.GetData<PlayerData>("Child");
        PlayerData parent = GameData.GetData<PlayerData>("Parent");

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
        timeCoroutine = StartCoroutine(TimeCounter());
    }

    private IEnumerator TimeCounter()
    {
        while (playTimeData.RemainingPlayTimeInt>=0)
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