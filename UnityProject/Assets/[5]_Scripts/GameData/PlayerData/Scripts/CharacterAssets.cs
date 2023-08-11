using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/CharacterAssetContainer")]
public class CharacterAssets : AData
{
    public AssetContainer male, female;
    private Gender gender = Gender.Female;

    public AssetContainer GetContainer()
    {
        AssetContainer container;
        if (gender == Gender.Male)
        {
            container = male;
            if (container.prefab == null)
            {
                return female;
            }
        }
        else
        {
            container = female;
            if (container.prefab == null)
            {
                return male;
            }
        }
        return container;
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
