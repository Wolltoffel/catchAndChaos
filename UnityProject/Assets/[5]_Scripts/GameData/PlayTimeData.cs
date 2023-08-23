using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayTimeRemaining")]
public class PlayTimeData : StaticData
{
    public bool hasMotherWon { get => hasGameEnded && !hasChildWon; }
    public bool hasChildWon;
    public bool hasGameEnded;

    [SerializeField] private int playTime; 
    private float tempRemainingPlayTime;

    public int RemainingPlayTimeInt { get => Mathf.FloorToInt(tempRemainingPlayTime); }
    public float TempRemainingPlayTime 
    {
        get => tempRemainingPlayTime; 
        set => tempRemainingPlayTime = Mathf.Clamp(value,0,999);
    }

    public void ResetValues()
    {
        tempRemainingPlayTime = playTime;
        hasChildWon = false;
        hasGameEnded = false;
    }
}
