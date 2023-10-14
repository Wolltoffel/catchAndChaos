using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UICanvas;

    [Header("UI Element Managers")]
    [SerializeField] private SlideCooldownUI slideCooldown;
    [SerializeField] private ChaosMeterUI chaosMeter;
    [SerializeField] private TimeCounterUI timeCounter;
    [SerializeField] private PlayerScoreUI parentScore;
    [SerializeField] private PlayerScoreUI childScore;

    private PlayTimeData timeData;
    private ChaosData chaosData;
    private ChildData childData;
    private ParentData parentData;

    private void Start()
    {
        UICanvas.SetActive(true);
        timeData = GameData.GetData<PlayTimeData>("PlayTimeData");
        chaosData = GameData.GetData<ChaosData>("ChaosData");
        childData = GameData.GetData<ChildData>("Child");
        parentData = GameData.GetData<ParentData>("Parent");
        UpdatePlayerScores();
    }

    private void Update()
    {
        UpdateTimeCounterVisual();
        UpdateChaosMeter();
        UpdateSlideCooldown();
    }

    public void UpdatePlayerScores()
    {
        parentScore.UpdateUI(parentData.tempScore);
        childScore.UpdateUI(childData.tempScore);
    }

    /// <summary>
    /// Sends the TimeCounter the current data
    /// </summary>
    private void UpdateTimeCounterVisual()
    {
        timeCounter.UpdateUI(timeData.RemainingPlayTimeInt);
    }

    /// <summary>
    /// Sends the SlideCooldown the current data
    /// </summary>
    private void UpdateSlideCooldown()
    {
        slideCooldown.UpdateUI(childData.tempSlideCoolDown);
    }

    /// <summary>
    /// Sends the ChaosMeter the current data
    /// </summary>
    private void UpdateChaosMeter()
    {
        chaosMeter.UpdateUI(chaosData.CurrentChaos);
    }
}