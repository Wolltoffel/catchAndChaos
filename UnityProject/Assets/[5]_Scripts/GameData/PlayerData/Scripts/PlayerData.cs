using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerData : StaticData
{
     public int tempScore;
    [HideInInspector] public string tempInputDevice = "K1";
    [HideInInspector] public bool hasWon = false;

    public CharacterAssets characterAssets;

}
