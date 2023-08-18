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
    private PlayTimeRemaining timeData;
    private GameInteractableManager interactableManager;

    ////EndGame
    //public static bool hasEnded;

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
        timeData = GameData.GetData<PlayTimeRemaining>("PlayTimeRemaining");
        chaosData.ResetValues();

        //Set up time counter
        SetupTimeCounter();

        //Time.timeScale = 1;
        //hasEnded = false;
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
        switch (condition)
        {
            case EndCondition.Catch | EndCondition.Time:
                GameData.GetData<PlayerData>("Parent").tempScore++;
                break;
            case EndCondition.Chaos:
                GameData.GetData<PlayerData>("Child").tempScore++;
                break;
        }

        ScreenSwitcher.AddScreen(ScreenType.ScoreInterim);

        //Time.timeScale = 0;
    }

    #region Timecounter
    private void SetupTimeCounter()
    {
        ResetTimeCounter();
        timeCoroutine = StartCoroutine(TimeCounter());
    }

    private IEnumerator TimeCounter()
    {
        while (timeData.RemainingPlayTimeInt>=0)
        {
            yield return new WaitForSeconds(1);
            timeData.TempRemainingPlayTime--;
        }

        EndGame(EndCondition.Time);
    }

    private void ResetTimeCounter()
    {
        timeData.ResetValues();
    }
    #endregion
}

public enum EndCondition
{
    Chaos,
    Catch,
    Time
}