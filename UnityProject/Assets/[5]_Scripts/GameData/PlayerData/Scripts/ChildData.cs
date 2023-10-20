using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ChildData")]
public class ChildData : PlayerData
{
    
    [Header ("Data")]
    [HideInInspector] public int tempSlideCoolDown = 0;
    [HideInInspector] public bool stunned;

    private float slideCoolDown = 0;
    public float SlideCoolDown { get { return slideCoolDown; } set { hasReturnedTrue = false; slideCoolDown = value; } }

    public float stunTime;
    public float lollyDuration;
    public float lollySpeed;
    public float defaultSpeed;
    [HideInInspector] public float tempSpeed;
    public float timeToDestroy;
    public int chaosScorePerChaosObject;
    public bool isCatchable = true;

    private bool hasReturnedTrue = true;
    public bool IsSlideReady { get {
            if (SlideCoolDown > 0)
            {
                return false;
            }
            if (hasReturnedTrue)
            {
                return true;
            }
            hasReturnedTrue = true;
            return false;
        } set => hasReturnedTrue = value; }

    public GameObject destroyParticles;

    public void ResetData()
    {
        tempSlideCoolDown = 0;
        stunned = false;
        tempSpeed = defaultSpeed;
    }
}
