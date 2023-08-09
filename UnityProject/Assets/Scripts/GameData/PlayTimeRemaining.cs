using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayTimeRemaining")]
public class PlayTimeRemaining : AData
{
    [SerializeField] private int playTime; 
    public float remainingPlayTime;
    public int RemainingPlayTimeInt { get => Mathf.FloorToInt(remainingPlayTime); }

    public void ResetValues()
    {
        remainingPlayTime = playTime;
    }
}
