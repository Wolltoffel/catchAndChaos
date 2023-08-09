using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Element Managers")]
    [SerializeField] private SlideCooldown slideCooldown;
    [SerializeField] private ChaosMeter chaosMeter;
    [SerializeField] private TimeCounter timeCounter;
    [SerializeField] private PlayerScores playerScores;

    IEnumerator Start()
    {
        var temp = GameData.GetData<PlayTimeRemaining>("PlayTimeRemaining");
        temp.ResetValues();

        while (true)
        {
            timeCounter.UpdateUI(temp.RemainingPlayTimeInt);

            yield return new WaitForSeconds(1);

            temp.remainingPlayTime--;
        }
    }
}