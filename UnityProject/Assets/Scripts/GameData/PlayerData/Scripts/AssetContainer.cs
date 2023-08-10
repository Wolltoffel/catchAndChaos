using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkinMaterials
{
    public Material body, face;
}

public enum Gender
{
    Male, Female
}

public enum Step
{
    Prev, Next
}


[System.Serializable]
public struct AssetContainer
{
    public GameObject prefab;

    [Header ("Materials")]
    public Material  clothing;
    public Material []hair;
    public SkinMaterials[] skinMaterials;

    [Header ("Properties")]
    private int tempActiveHairIndex;
    private int tempActiveSkinIndex;

    public int GetActiveHairIndex()
    {
         return tempActiveHairIndex;
    }

     public void UpdateActiveHairIndex(Step step)
    {
        int numberOfHairMaterials = hair.Length;

        if (step == Step.Next)
            tempActiveHairIndex = (tempActiveHairIndex+1)%numberOfHairMaterials;
        else
            tempActiveHairIndex = (tempActiveHairIndex-1+ numberOfHairMaterials)%numberOfHairMaterials;
    }
    public int GetActiveSkinIndex()
    {
        return tempActiveSkinIndex;
    }

    public void UpdateActiveSkinIndex(Step step)
    {   
        int numberOfSkinMaterials = skinMaterials.Length;

         if (step == Step.Next)
            tempActiveSkinIndex = (tempActiveSkinIndex+1)%numberOfSkinMaterials;
        else
            tempActiveHairIndex = (tempActiveHairIndex-1+ numberOfSkinMaterials)%numberOfSkinMaterials;
    }
    public Material[] GetActiveMaterials()
    {   
        Material [] materials = new Material[4];
        materials[0] = hair[tempActiveHairIndex]; //Hair
        materials[1] = skinMaterials[tempActiveSkinIndex].face; //Face
        materials[1] = skinMaterials[tempActiveSkinIndex].body;

        return materials;
    }

}
