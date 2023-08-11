using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{   
    [SerializeField] public Characters character;
    [SerializeField] public GameObject placeholder;
    Transform parent;
    Material[] materials;
    string searchkey;

    public void Start()
    {
        parent = placeholder.transform.parent;

        SetSearchKey();

        SpawnCharacter();
    }

    private void ApplyMaterials()
    {    
        ApplyGender();
        materials = GameData.GetData<PlayerData>(searchkey).characterAssets.GetActiveMaterials();
        placeholder.GetComponentInChildren<SkinnedMeshRenderer>().materials = materials;
    }

    private void ApplyGender()
    {
        Vector3 position = placeholder.transform.position;
        Destroy(placeholder);
        GameObject spawnableObject = GameData.GetData<PlayerData>(searchkey).characterAssets.GetContainer().prefab;
        placeholder = Instantiate (spawnableObject, position, Quaternion.identity);
        placeholder.transform.parent = parent;
        placeholder.name = searchkey + "_Spawned";
    }

    public void SetData()
    {
        var temp = GameData.GetData<PlayerData>(searchkey);
    }

    public void SpawnCharacter()
    {
        ApplyGender();
        ApplyMaterials();
        SetData();
    }

    public void SetSearchKey()
    {
        if (character == Characters.Child)
            searchkey = "Child";
        else
            searchkey = "Parent";
    }
}
