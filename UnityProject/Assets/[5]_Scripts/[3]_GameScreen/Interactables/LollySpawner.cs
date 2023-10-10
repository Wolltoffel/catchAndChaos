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
    List<Transform> spawnPositions = new List<Transform>();
    List<Transform> originalPositions = new List<Transform>();

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

        InteractableContainer interactableData = GameData.GetData<InteractableContainer>("InteractableContainer");
        Debug.Log(interactableData);

        lollyProduction = StartCoroutine(SpawnLollysOverTime());
    }

    private IEnumerator SpawnLollysOverTime()
    {
        PlayTimeData playTimeData = GameData.GetData<PlayTimeData>("PlayTimeData");
        bool shouldSpawn = false;

        while (!playTimeData.hasGameEnded)
        {

            Debug.Log("Waiting");

            yield return new WaitForSeconds(lollySpawnRate);

            InteractableContainer interactableData = GameData.GetData<InteractableContainer>("InteractableContainer");
            shouldSpawn = true;

            Debug.Log(!playTimeData.hasGameEnded);
            Debug.Log(shouldSpawn);
            Debug.Log(interactableData);

            while (!playTimeData.hasGameEnded && shouldSpawn)
            {
                Debug.Log("Spawing");
                if (interactableData.GetInteractableCategory("Lolly").objects.Count < maxAmountOfLollys)
                {
                    var objects = GetFreeLollySpawn();
                    var spawnedLolly = SpawnSingleLolly(objects[Random.Range(0,objects.Length)].position);
                    interactableData.AddObjectToCategory("Lolly", spawnedLolly);
                    shouldSpawn = false;
                    Debug.Log("Spawned");
                }
                else
                {
                    Debug.Log("Failed");
                    yield return new WaitForSeconds(3);
                }
            }
        }

        lollyProduction = null;
        yield break;
    }

    private Transform[] GetFreeLollySpawn()
    {
        var data = GameData.GetData<InteractableContainer>("InteractableContainer");
        var category = data.GetInteractableCategory("Lolly").objects.ToArray();

        spawnPositions = originalPositions;
        foreach (var spawnPoint in originalPositions)
        {
            foreach (var item in category)
            {
                if (spawnPoint.position == item.transform.position)
                {
                    spawnPositions.Remove(spawnPoint);
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
            spawnPositions.Add (spawnDummies[i].transform);
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
        Vector3 randomPos = spawnPositions[random].position;

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
