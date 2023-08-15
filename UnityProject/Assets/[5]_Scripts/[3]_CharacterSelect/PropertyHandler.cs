using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Characters
{
    Child, Parent
}

public class PropertyHandler
{
    public static void SwitchProperty (Characters character,Step step)
    {

        if (character == Characters.Child)
            GameData.GetData<PlayerData>("Child").characterAssets.UpdateCharacterPrefab(step);
        else if (character == Characters.Parent)
            GameData.GetData<PlayerData>("Parent").characterAssets.UpdateCharacterPrefab(step);
    }

    public static void SetProperty(Characters characters,int valueInt)
    {
          if (characters == Characters.Child)
            GameData.GetData<PlayerData>("Child").characterAssets.SetActivePrefabIndex(valueInt);
        else if (characters == Characters.Parent)
            GameData.GetData<PlayerData>("Parent").characterAssets.SetActivePrefabIndex(valueInt);
    }

}
