using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/CharacterAssetContainer")]
public class CharacterAssets : AData
{
    public AssetContainer male, female;
    private Gender gender = Gender.Male;

    public AssetContainer GetContainer()
    {
        if (gender == Gender.Female)
            return female;
        else return male;
    }
    public Material[] GetActiveMaterials()
    {   
        AssetContainer assetContainer = GetContainer();
        return assetContainer.GetActiveMaterials();
    }

    public void UpdateActiveSkinIndex(Step step)
    {   
        AssetContainer assetContainer = GetContainer();
        assetContainer.UpdateActiveSkinIndex(step);
    }


    public void UpdateActiveHairIndex(Step step)
    {
        AssetContainer assetContainer = GetContainer();
        assetContainer.UpdateActiveHairIndex(step);
    }

    public void SwitchGender()
    {
        if (gender == Gender.Female)
            gender = Gender.Male;
        else
            gender = Gender.Female;
    }

}
