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
    None,Male, Female
}

public enum Step
{
    Prev, Next
}


[System.Serializable]
public class AssetContainer
{    public GameObject prefab;

    [Header ("Materials")]
    public Material  clothing;
    public Material []hair;
    public SkinMaterials[] skinMaterials;



    public void UpdateActiveHairIndex(Step step, int activeHairIndex, out int newActiveHairIndex)
    {
        int numberOfHairMaterials = hair.Length;
        if (step == Step.Next){
             activeHairIndex = (activeHairIndex+1)%numberOfHairMaterials;
        }    
        else
            activeHairIndex = (activeHairIndex-1+ numberOfHairMaterials)%numberOfHairMaterials;

        newActiveHairIndex = activeHairIndex;
    }

    public void SetActiveHairIndex (int newActiveHairIndex, out int updatedActiveHairIndex)
    {
        if (newActiveHairIndex>hair.Length-1)
            throw new System.Exception ("NewActiveHairIndex exceeds number of materials");
        else
            updatedActiveHairIndex = newActiveHairIndex;
    }
    public void UpdateActiveSkinIndex(Step step, int activeSkinIndex, out int newActiveSkinIndex)
    {   
        int numberOfSkinMaterials = skinMaterials.Length;

        Debug.Log (activeSkinIndex +" Skin Index before");

         if (step == Step.Next)
            activeSkinIndex = (activeSkinIndex+1)%numberOfSkinMaterials;
        else
            activeSkinIndex = (activeSkinIndex-1+ numberOfSkinMaterials)%numberOfSkinMaterials;

        newActiveSkinIndex = activeSkinIndex;
        Debug.Log (activeSkinIndex +" Skin Index after");
    }

    public void SetActiveSkinIndex (int newActiveSkinIndex, out int updatedActiveSkinIndex)
    {
        if (newActiveSkinIndex>skinMaterials.Length-1)
            throw new System.Exception ("NewActiveSkinIndex exceeds number of materials");
        else
            updatedActiveSkinIndex = newActiveSkinIndex;
    }

    public Material[] GetActiveMaterials(int activeHairIndex,int activeSkinIndex)
    {   
        Material [] materials = new Material[4];
        materials[0] = hair[activeHairIndex]; //Hair
        materials[1] = skinMaterials[activeSkinIndex].face; //Face
        materials[2] = skinMaterials[activeSkinIndex].body; //Body
        materials[3] = clothing;

        return materials;
    }

}
