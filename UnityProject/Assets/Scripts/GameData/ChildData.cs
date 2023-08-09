using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent/ChildData")]
public class ChildData : PlayerAgent
{
    
    [Header ("Customisation")]
    [HideInInspector] public string tempSkinColor;

    public CharacterAssetContainer skinContainerBoy,skinContainerGirl;
    [HideInInspector] public int tempSlideCoolDown;
}
