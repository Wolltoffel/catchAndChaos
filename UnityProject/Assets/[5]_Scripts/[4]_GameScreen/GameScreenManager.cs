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
            Endgame(EndCondition.Chaos);
        }
    }

    public static void Endgame(EndCondition condition)
    {
        Debug.Log($"Game Has Ended due to {condition}");
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

        Endgame(EndCondition.Time);
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