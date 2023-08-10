using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialApplier : MonoBehaviour
{   
    [SerializeField] Character character;
    [SerializeField] GameObject characterPrefab;
    Material[] materials;
    string searchkey;
   void Start()
   {
        materials = characterPrefab.GetComponentInChildren<SkinnedMeshRenderer>().materials;
        
        if (character == Character.Child)
            searchkey = "Child";
        else
            searchkey = "Parent";

        ApplyMaterials();
   }

   public void ApplyMaterials()
   {    
        ApplyGender();
        materials = GameData.GetData<PlayerData>(searchkey).characterAssets.GetActiveMaterials();
        characterPrefab.GetComponentInChildren<SkinnedMeshRenderer>().materials = materials;

   }

   public void ApplyGender()
   {
        Destroy(characterPrefab);
        GameObject spawnableObject = GameData.GetData<PlayerData>(searchkey).characterAssets.GetContainer().prefab;
        characterPrefab = Instantiate (spawnableObject);
        characterPrefab.name = "BodyTest";
   }
}
