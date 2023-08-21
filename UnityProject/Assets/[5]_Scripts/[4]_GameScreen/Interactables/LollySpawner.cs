using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LollySpawner : MonoBehaviour
{
    [SerializeField]GameObject lollyPrefab;
    [SerializeField]int amountOfLollys;
    [SerializeField] GameObject parentObject;
    List <Transform> spawnPositions = new List<Transform>();
    
    public void Start()
    {
        GetAllPotentialLollyPos();
        SpawnLollys();
    }

    public void GetAllPotentialLollyPos()
    {
        GameObject[] spawnDummies =  GameObject.FindGameObjectsWithTag("LollySpawnPoint");

        for (int i = 0; i<spawnDummies.Length; i++)
        {
            spawnPositions.Add (spawnDummies[i].transform);
            Destroy(spawnDummies[i]);
        }
}

    public void SpawnLollys()
    {       
        for (int i = 0; i<amountOfLollys; i++)
        {
            SpawnLolly();
        }
    }

    void SpawnLolly()
    {
        //Get random transform
        int random = Random.Range(0,spawnPositions.Count);
        Vector3 randomPos = spawnPositions[random].position;

        GameObject spawnedLolly =  Instantiate(lollyPrefab,randomPos,Quaternion.identity);
        spawnedLolly.transform.name = "LollyItem";
        spawnedLolly.transform.parent = parentObject.transform;
        spawnPositions.Remove(spawnPositions[random]);
    }
}
