using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Element Managers")]
    [SerializeField] private SlideCooldownUI slideCooldown;
    [SerializeField] private ChaosMeterUI chaosMeter;
    [SerializeField] private TimeCounterUI timeCounter;

    private PlayTimeRemaining timeData;
    private ChaosData chaosData;
    private ChildData childData;

    private void Awake()
    {
        timeData = GameData.GetData<PlayTimeRemaining>("PlayTimeRemaining");
        chaosData = GameData.GetData<ChaosData>("ChaosData");
        childData = GameData.GetData<ChildData>("Child");
    }

    private void Update()
    {
        UpdateTimeCounterVisual();
        UpdateChaosMeter();
        UpdateSlideCooldown();
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