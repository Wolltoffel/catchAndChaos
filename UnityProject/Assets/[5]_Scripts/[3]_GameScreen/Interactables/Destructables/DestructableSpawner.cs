using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSpawner : MonoBehaviour
{
    List<GameObject> potentialDestructables  = new List<GameObject>();
    [SerializeField] int amountOfDestructables;
    [SerializeField]Color outlineColor;

    [Header ("Highlight")]
    [SerializeField] Shader highlightShader;
    [SerializeField] Color emissionColor;
    [SerializeField] float emissionStrength;
    
    
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
        potentialDestructables.Remove(potentialDestructables[random]);
        randomDestructable.AddComponent<Destructable>();
        randomDestructable.tag = "Chaos";
        ActivateHighlight(randomDestructable);
        Debug.Log(randomDestructable.name);
    }

    public void ActivateHighlight(GameObject target)
    {
       Material[] materials = target.GetComponent<Renderer>().materials;
       for (int i = 0; i < materials.Length; i++)
       {    
            target.GetComponent<Destructable>().prevShader = materials[i].shader;
            Color baseColor = materials[i].GetColor("_BaseColor");
            materials[i].shader = highlightShader;
            materials[i].SetColor("_EmissionColor", emissionColor*emissionStrength);
            materials[i].SetColor("_Color",baseColor);
       }
    }
}
