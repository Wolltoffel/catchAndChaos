using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ChildData")]
public class ChildData : PlayerData
{
    
    [Header ("Data")]
    [HideInInspector] public int tempSlideCoolDown = 0;
    [HideInInspector] public bool stunned;
    public float slideCoolDown;
    public float stunTime;
    public float lollyDuration;
    public float lollySpeed;
    public float defaultSpeed;
    [HideInInspector] public float tempSpeed;
    public float timeToDestroy;
    public int chaosScorePerChaosObject;

}
