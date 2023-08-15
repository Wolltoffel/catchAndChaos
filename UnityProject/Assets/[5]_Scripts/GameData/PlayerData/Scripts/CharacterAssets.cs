using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Step
{
    Prev, Next
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/CharacterAssetContainer")]
public class CharacterAssets : StaticData
{
    [HideInInspector]public GameObject prefab
    {
        get
        {
            return characterPrefab[activePrefabIndex];

        }
    }

    [SerializeField] GameObject[] characterPrefab;

    int activePrefabIndex;


    public void UpdateCharacterPrefab(Step step)
    {
        int numberOfPrefabs = characterPrefab.Length;
        if (step == Step.Next){
             activePrefabIndex = (activePrefabIndex+1)%numberOfPrefabs;
        }    
        else
            activePrefabIndex = (activePrefabIndex-1+ numberOfPrefabs)%numberOfPrefabs;
    }

    public void SetActivePrefabIndex (int newActivePrefabIndex)
    {
        if (newActivePrefabIndex>characterPrefab.Length-1)
            throw new System.Exception ("NewActiveHairIndex exceeds number of materials");
        else
            activePrefabIndex = newActivePrefabIndex;
    }

}
