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

    public void SetProperty (Characters character,ModelProperty modelProperty, Step step)
    {
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
