using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSpawner : MonoBehaviour
{
    List<GameObject> potentialDestructables  = new List<GameObject>();
    [SerializeField] int amountOfDestructables;
    [SerializeField] Material outlineMaterial;
    GameObject randomDestructable;

    public void Awake()
    {
        GetAllPotentialDestructibles();
        SetDestructables();
    }
    public void GetAllPotentialDestructibles()
    {
        GameObject[] destructables = GameObject.FindGameObjectsWithTag("Chaos");

        for (int i = 0; i < destructables.Length; i++)
        {
            potentialDestructables.Add(destructables[i]);
            destructables[i].tag = "Untagged";
        }

    }

    public void SetDestructables()
    {
        for (int i = 0; i < amountOfDestructables; i++)
        {
            SpawnDestructable();
        }
    }

    void SpawnDestructable()
    {
        //Get random destructable
        int random = Random.Range(0, potentialDestructables.Count);
        randomDestructable = potentialDestructables[random];
        randomDestructable.tag = "Chaos";
        ActivateOutline(randomDestructable);
        potentialDestructables.Remove(potentialDestructables[random]);
        randomDestructable.AddComponent<Destructable>();
        Debug.Log(randomDestructable.name);
    }

    public void ActivateOutline(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        Material[] oldMaterials = renderer.materials;
        Material[] newMaterials = new Material[2];
        newMaterials[0] = renderer.materials[0];
        newMaterials[1] = outlineMaterial;
        renderer.materials = newMaterials;
    }

    public void DeactivateOutline(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        Material[] oldMaterials = renderer.materials;
        Material[] newMaterials = new Material[1];
        newMaterials[0] = renderer.materials[0];
        renderer.materials = newMaterials;
    }
}
