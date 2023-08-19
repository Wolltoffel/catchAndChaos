using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayTimeRemaining")]
public class PlayTimeData : StaticData
{
    [SerializeField] private int playTime;
    private float tempRemainingPlayTime;
    public bool hasMotherWon;
    public bool hasChildWon;
    public bool HasGameEnded { get => hasChildWon || hasMotherWon; }

    public int RemainingPlayTimeInt { get => Mathf.FloorToInt(tempRemainingPlayTime); }
    public float TempRemainingPlayTime 
    {
        get => tempRemainingPlayTime; 
        set => tempRemainingPlayTime = Mathf.Clamp(value,0,999);
    }

    public void ResetValues()
    {
        tempRemainingPlayTime = playTime;
    }
}
