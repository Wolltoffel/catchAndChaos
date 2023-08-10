using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModelProperty
{
    Hair, Gender, Skincolor
}

public enum Character
{
    Child, Parent
}

public class PropertyHandler
{
    GameObject childBody;
    GameObject parentBody;

    public PropertyHandler(){}

    public void SetProperty (Character character,ModelProperty modelProperty, Step step)
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
    }

    public void ApplyProperties(Character character, GameObject body)
    {
        CharacterAssets characterAssets = GetCharacterAssets(character);
        body.GetComponent<SkinnedMeshRenderer>().materials = characterAssets.GetActiveMaterials();
    }

    CharacterAssets GetCharacterAssets(Character character)
    {
        string searchKey = "";

        if (character == Character.Child)
            searchKey = "Child";
        else
            searchKey = "Parent";

        PlayerData playerData = GameData.GetData<PlayerData>(searchKey);
        return playerData.characterAssets;
    }

}
