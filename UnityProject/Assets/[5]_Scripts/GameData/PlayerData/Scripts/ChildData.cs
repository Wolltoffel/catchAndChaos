using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ChildData")]
public class ChildData : PlayerData
{
    
    [Header ("Data")]
    [HideInInspector] public int tempSlideCoolDown = 0;
    public bool isStunned;
}
