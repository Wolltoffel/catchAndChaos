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

    private void Update()
    {
        UpdateTimeCounterVisual();
        UpdateChaosMeter();
        //UpdateSlideCooldown();
    }

    /// <summary>
    /// Sends the TimeCounter the current data
    /// </summary>
    private void UpdateTimeCounterVisual()
    {
        PlayTimeRemaining data = GameData.GetData<PlayTimeRemaining>("PlayTimeRemaining");
        timeCounter.UpdateUI(data.RemainingPlayTimeInt);
    }

    /// <summary>
    /// Sends the SlideCooldown the current data
    /// </summary>
    private void UpdateSlideCooldown()
    {
        //finsih after merge
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends the ChaosMeter the current data
    /// </summary>
    private void UpdateChaosMeter()
    {
        ChaosData data = GameData.GetData<ChaosData>("ChaosData");
        chaosMeter.UpdateUI(data.CurrentChaos);
    }
}