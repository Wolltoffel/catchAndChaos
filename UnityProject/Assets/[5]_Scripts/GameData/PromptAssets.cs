using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PromptAssetContainer
{
    public GameObject promptPrefab;
    public string name;
}


[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PromptAssets")]
public class PromptAssets : StaticData
{
    public PromptAssetContainer[] promptAssetContainers;

    public GameObject GetPromptAssetByName(string name)
    {
        for (int i = 0; i < promptAssetContainers.Length; i++)
        {
            if (promptAssetContainers[i].name == name)
                return promptAssetContainers[i].promptPrefab;

        }
        Debug.Log ("No such prompt found!");
        return null;
    }
}
