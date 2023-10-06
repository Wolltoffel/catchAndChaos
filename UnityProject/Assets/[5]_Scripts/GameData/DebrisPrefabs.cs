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

        float totalProbability = 0;
        for (int i = 0; i < debris.Length; i++)
        {
            totalProbability += debris[i].probablility;
        }

        int j = 0;
        int maxLoops = 5;
        while (true)
        {
            float random = Random.Range(0, 1f);
            if (debris[j].probablility/totalProbability > random || maxLoops <= 0)
            {
                gameObjects.Add(debris[j].debris);
                if (gameObjects.Count >= 3)
                    break;
            }

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
    public float probablility = 1;
}