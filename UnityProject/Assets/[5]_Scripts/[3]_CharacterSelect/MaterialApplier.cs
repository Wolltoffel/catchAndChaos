using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialApplier : MonoBehaviour
{   
    [SerializeField] Character character;
    [SerializeField] GameObject placeholder;
    Transform parent;
    Material[] materials;
    string searchkey;
   void Start()
   {
        parent = placeholder.transform.parent;

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
        placeholder.GetComponentInChildren<SkinnedMeshRenderer>().materials = materials;

   }

   public void ApplyGender()
   {
        Vector3 position = placeholder.transform.position;
        Destroy(placeholder);
        GameObject spawnableObject = GameData.GetData<PlayerData>(searchkey).characterAssets.GetContainer().prefab;
        placeholder = Instantiate (spawnableObject, position, Quaternion.identity);
        placeholder.transform.parent = parent;
        placeholder.name = "BodyTest";
   }
}
