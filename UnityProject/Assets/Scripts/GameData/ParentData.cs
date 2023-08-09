using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ParentData")]

public class ParentData : PlayerAgent
{
    [Header ("Customisation")]
    [HideInInspector] public string tempSkinColor;
     public CharacterAssetContainer skinContainer;
}
