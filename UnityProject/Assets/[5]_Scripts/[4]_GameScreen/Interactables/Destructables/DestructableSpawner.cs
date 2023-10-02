using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSpawner : MonoBehaviour
{
    List<GameObject> potentialDestructables  = new List<GameObject>();
    [SerializeField] int amountOfDestructables;
    [SerializeField]Color outlineColor;
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
        CustomOutline customOutline = target.GetComponent<CustomOutline>();
        if (customOutline==null)
            customOutline = target.AddComponent<CustomOutline>();

        customOutline.activeOutline = true;
        customOutline.outlineColor = outlineColor;
        customOutline.outlineWidth = 3f;
        customOutline.StartPulsating(20,30,Effect.OutlineWidth,2f);
        customOutline.seeThroughWalls = true;
    }

    public void DeactivateOutline(GameObject target)
    {
        target.layer = 0;
        //Set Children
        Transform[] children = target.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.layer = 0;
        }
    }
}
