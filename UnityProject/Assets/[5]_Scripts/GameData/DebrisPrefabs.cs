using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebrisPrefabs", menuName = "Custom/AData/DebrisPrefabs")]
public class DebrisPrefabs : StaticData
{
    public Debris[] debris;

    public GameObject[] GetDebris(DebrisTypes debrisType)
    {
        List<GameObject> prefabs = new List<GameObject>();

        for (int i = 0; i < debris.Length; i++)
        {
            if (debris[i].debrisType == debrisType)
            {
                prefabs.Add(debris[i].debris);
            }
        }

        return prefabs.ToArray();
    }
}

[System.Serializable]
public class Debris
{
    public GameObject debris;
    public DebrisTypes debrisType = DebrisTypes.Shelf;
}