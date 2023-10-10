using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LollySpawner : MonoBehaviour
{
    [SerializeField]GameObject lollyPrefab;
    [SerializeField]int amountOfLollys;
    [Tooltip("Seconds after which a new lolly spawns")]
    [SerializeField] float lollySpawnRate;
    [SerializeField] int maxAmountOfLollys;
    [SerializeField] GameObject parentObject;
    List<Vector3> spawnPositions = new List<Vector3>();
    List<Vector3> originalPositions = new List<Vector3>();

    Coroutine lollyProduction;
    
    public void Awake()
    {
        
    }

    private void Start()
    {
        GetAllPotentialLollyPos();
        SpawnLollys();

        if (lollyProduction != null)
        {
            StopCoroutine(lollyProduction);
            lollyProduction = null;
        }

        lollyProduction = StartCoroutine(SpawnLollysOverTime());
    }

    private IEnumerator SpawnLollysOverTime()
    {
        PlayTimeData playTimeData = GameData.GetData<PlayTimeData>("PlayTimeData");
        bool shouldSpawn = false;

        while (!playTimeData.hasGameEnded)
        {

            //Debug.Log("Waiting");

            yield return new WaitForSeconds(lollySpawnRate);

            shouldSpawn = true;

            while (!playTimeData.hasGameEnded && shouldSpawn)
            {
                InteractableContainer interactableData = GameData.GetData<InteractableContainer>("InteractableContainer");

                //Debug.Log("Spawing");
                if (interactableData.GetInteractableCategory("Lolly").objects.Count < maxAmountOfLollys)
                {
                    var objects = GetFreeLollySpawn();
                    var spawnedLolly = SpawnSingleLolly(objects[Random.Range(0,objects.Length)]);
                    interactableData.AddObjectToCategory("Lolly", spawnedLolly);
                    shouldSpawn = false;
                    //Debug.Log("Spawned");
                }
                else
                {
                    //Debug.Log("Failed");
                    yield return new WaitForSeconds(3);
                }
            }
        }

        lollyProduction = null;
        yield break;
    }

    private Vector3[] GetFreeLollySpawn()
    {
        var data = GameData.GetData<InteractableContainer>("InteractableContainer");
        var category = data.GetInteractableCategory("Lolly").objects.ToArray();

        spawnPositions = originalPositions;
        for (int i = 0; i < originalPositions.Count; i++)
        {
            for (int j = 0; j < category.Length; j++)
            {
                if (spawnPositions[i] == null)
                {
                    break;
                }
                if (spawnPositions[i] == category[j].transform.position)
                {
                    spawnPositions.Remove(spawnPositions[i]);
                    break;
                }
            }
        }

        return spawnPositions.ToArray();
    }

    public void GetAllPotentialLollyPos()
    {
        GameObject[] spawnDummies =  GameObject.FindGameObjectsWithTag("LollySpawnPoint");

        for (int i = 0; i<spawnDummies.Length; i++)
        {
            spawnPositions.Add (spawnDummies[i].transform.position);
            originalPositions = spawnPositions;
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
        Vector3 randomPos = spawnPositions[random];

        SpawnSingleLolly(randomPos);

        spawnPositions.Remove(spawnPositions[random]);
    }

    GameObject SpawnSingleLolly(Vector3 spawnPosition)
    {
        GameObject spawnedLolly = Instantiate(lollyPrefab, spawnPosition, Quaternion.identity);
        spawnedLolly.transform.name = "LollyItem";
        spawnedLolly.transform.parent = parentObject.transform;
        return spawnedLolly;
    }
}
