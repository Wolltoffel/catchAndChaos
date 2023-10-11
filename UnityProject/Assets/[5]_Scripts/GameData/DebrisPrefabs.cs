using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebrisPrefabs", menuName = "Custom/AData/DebrisPrefabs")]
public class DebrisPrefabs : StaticData
{
    public Debris[] debris;

    public GameObject[] GetRandomDebris()
    {
        List<GameObject> gameObjects = new List<GameObject>();

        int j = 0;
        int maxLoops = 5;
        while (true)
        {

            gameObjects.Add(debris[j].debris);
            if (gameObjects.Count >= 3)
                break;

            j = (j + 1) % debris.Length;
            maxLoops--;
        }

        return gameObjects.ToArray();
    }
}

[System.Serializable]
public class Debris
{
    public GameObject debris;
    public DebrisTypes debrisType = DebrisTypes.Shelf;
}