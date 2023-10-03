using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Step
{
    Prev, Next
}

[Serializable]
public class CharacterAssetItem
{
    public GameObject characterPrefab;
    public Sprite characterPortrait;
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/CharacterAssetContainer")]
public class CharacterAssets : StaticData
{
    [HideInInspector]public GameObject prefab
    {
        get
        {
            return characterAssetItems[activePrefabIndex].characterPrefab;

        }
    }

    [SerializeField] CharacterAssetItem [] characterAssetItems;
    int activePrefabIndex;


    public void UpdateCharacterPrefab(Step step)
    {
        int numberOfPrefabs = characterAssetItems.Length;
        if (step == Step.Next){
             activePrefabIndex = (activePrefabIndex+1)%numberOfPrefabs;
        }    
        else
            activePrefabIndex = (activePrefabIndex-1+ numberOfPrefabs)%numberOfPrefabs;
    }

    public void SetActivePrefabIndex (int newActivePrefabIndex)
    {
        if (newActivePrefabIndex>characterAssetItems.Length-1)
            throw new System.Exception ("NewActiveHairIndex exceeds number of materials");
        else
            activePrefabIndex = newActivePrefabIndex;
    }

    public int GetActivePrefabIndex()
    {
        return activePrefabIndex;
    }

    public CharacterAssetItem GetCharacterAssetItemAt(int index)
    {
        return characterAssetItems[index];
    }

   public CharacterAssetItem[] GetCharacterAssetItems()
   {
        return characterAssetItems;
   }


}
