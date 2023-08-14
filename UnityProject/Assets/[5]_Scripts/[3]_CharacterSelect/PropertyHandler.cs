using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModelProperty
{
    Hair, Gender, Skincolor
}

public enum Characters
{
    Child, Parent
}

public class PropertyHandler
{
    GameObject childBody;
    GameObject parentBody;

    public PropertyHandler(){}

    public void SwitchProperty (Characters character,ModelProperty modelProperty, Step step)
    {
        Debug.Log ("Setting new Property "+ modelProperty);

        CharacterAssets characterAssets = GetCharacterAssets(character);

        switch (modelProperty)
        {
            case ModelProperty.Hair:
                characterAssets.UpdateActiveHairIndex(step);
            break;
            case ModelProperty.Skincolor:
               characterAssets.UpdateActiveSkinIndex(step);
            break;
            case ModelProperty.Gender:
                characterAssets.SwitchGender();
            break;
        }

        ApplyProperties(character);
    }

    public void SetProperty(Characters characters, ModelProperty modelProperty, int valueInt = 999, Gender valueGender = Gender.None)
    {
        CharacterAssets characterAssets = GetCharacterAssets(characters);

         switch (modelProperty)
        {
            case ModelProperty.Hair:
                if (valueInt != 999)
                    characterAssets.SetActiveHairIndex(valueInt);
                else
                    throw new System.Exception ("No value specified");
            break;
            case ModelProperty.Skincolor:
                if (valueInt != 999)
                    characterAssets.SetActiveSkinIndex(valueInt);
                else
                    throw new System.Exception ("No value specified");
            break;
            case ModelProperty.Gender:
                if (valueGender != Gender.None)
                    characterAssets.SetGender(valueGender);
                else
                    throw new System.Exception ("No value specified");
            break;
        }

        ApplyProperties(characters);
    }

    public void ApplyProperties(Characters character)
    {
        CharacterAssets characterAssets = GetCharacterAssets(character);
        GameObject prefab = GetPlayerData(character).characterAssets.GetContainer().prefab;
        prefab.GetComponentInChildren<SkinnedMeshRenderer>().materials = characterAssets.GetActiveMaterials();
    }


    CharacterAssets GetCharacterAssets(Characters character)
    {
        PlayerData playerData = GetPlayerData(character);
        return playerData.characterAssets;
    }

    PlayerData GetPlayerData(Characters character)
    {
        string searchKey = "";

        if (character == Characters.Child)
            searchKey = "Child";
        else
            searchKey = "Parent";

        return GameData.GetData<PlayerData>(searchKey);
    }

}
