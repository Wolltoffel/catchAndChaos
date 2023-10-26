using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructableSpawner : MonoBehaviour
{
    List<GameObject> potentialDestructables  = new List<GameObject>();
    [SerializeField] int amountOfDestructables;

    [Header ("Highlight")]
    [SerializeField] Shader highlightShader;
    [SerializeField] Color emissionColor;
    [SerializeField] float emissionStrength;

    [SerializeField] Material highlightMaterial;
    
    
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
        if (randomDestructable==null)
            randomDestructable.AddComponent<Destructable>();
        randomDestructable.tag = "Chaos";
        ActivateHighlight(randomDestructable);
        Debug.Log(randomDestructable.name);
    }

    public void ActivateHighlight(GameObject target)
    {
       Material[] materialImport = target.GetComponent<Renderer>().materials;
       List<Material> materials  = new List<Material>();
       for (int i = 0; i < materialImport.Length; i++)
       {    
            materials.Add (materialImport[i]);
            
            //Material highlightMaterial = new Material(highlightShader);
            Material highlightMaterial = this.highlightMaterial;
            highlightMaterial.SetColor("_EmissionColor", emissionColor*emissionStrength);
            materials.Add (highlightMaterial);
       }

       target.GetComponent<Renderer>().SetMaterials(materials);
    }
}
