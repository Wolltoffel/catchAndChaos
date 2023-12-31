using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ParentData")]

public class ParentData : PlayerData
{
    public Transform handTransform;
    [HideInInspector] public Plushie plushie;
    
    public float catchDistance;
}
