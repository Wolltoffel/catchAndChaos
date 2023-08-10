using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenManager : MonoBehaviour
{
    //Parent and Child
    [SerializeField] private GameParent parent;
    [SerializeField] private GameChild child;

    //Audio
    [SerializeField] 
    private AudioClip[] backgroundAudioClips;

    //TimeCounter
    private float timeRemaining;
    private Coroutine timeCoroutine;

    //Data
    private ChaosData chaosData;
    private PlayTimeRemaining timeData;

    void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
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

    private void Update()
    {
        if (chaosData.EndGame)
        {
            Endgame(EndCondition.Chaos);
        }
    }

    private void Endgame(EndCondition condition)
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

enum EndCondition
{
    Chaos,
    Catch,
    Time
}