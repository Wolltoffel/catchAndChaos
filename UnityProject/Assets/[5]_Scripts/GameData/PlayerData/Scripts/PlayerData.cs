using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerData : StaticData
{
    [HideInInspector] public string tempScore;
    [HideInInspector] public string tempInputDevice = "K1";

    public CharacterAssets characterAssets;

}
