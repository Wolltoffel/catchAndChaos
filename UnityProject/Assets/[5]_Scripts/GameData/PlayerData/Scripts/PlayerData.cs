using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerData : StaticData
{
    [HideInInspector] public int tempScore;
    public string tempInputDevice = "K1";

    public CharacterAssets characterAssets;

    public void ResetTempScores()
    {
        tempScore = 0;
    }

}
