using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct SkinMaterials
{
    public Material body, face;
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/CharacterAssetContainer")]
public class CharacterAssetContainer : AData
{
    public Material  clothing;

    public Material []hair;
    public SkinMaterials[] skinMaterials; 

    private int tempActiveHairIndex;
    private int tempActiveSkinIndex;

    public GameObject prefab;

    public int GetActiveHairIndex()
    {
        return tempActiveHairIndex;
    }

    public void NextActiveHairIndex()
    {   
        int numberOfHairMaterials = hair.Length;
        tempActiveHairIndex = (tempActiveHairIndex+1)%numberOfHairMaterials;
    }

    public int GetActiveSkinIndex()
    {
        return tempActiveSkinIndex;
    }

    public void NextActiveSkinIndex()
    {   
        int numberOfSkinMaterials = skinMaterials.Length;
        tempActiveHairIndex = (tempActiveHairIndex+1)%numberOfSkinMaterials;
    }

}
