using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float destroyTimeLeft;

    public Shader prevShader;


    private void Awake()
    {
        destroyTimeLeft = GameData.GetData<ChildData>("Child").timeToDestroy;
    }

    public void DestroyObject()
    {
        GameData.GetData<InteractableContainer>("InteractableContainer").RemoveObjectFromCategory("Chaos", gameObject);
        GameData.GetData<ChaosData>("ChaosData").ModifyChaos(GameData.GetData<ChildData>("Child").chaosScorePerChaosObject);
        SpawnDebris(DebrisTypes.Shelf);
        Destroy(gameObject);
    }

    private void SpawnDebris(DebrisTypes type)
    {
        GameObject[] debris = GameData.GetData<DebrisPrefabs>("DebrisPrefabs").GetDebris(type);

        for (int i = 0; i < debris.Length*4; i++)
        {
            Instantiate(debris[i%debris.Length],transform.position + Vector3.up *0.5f,Quaternion.identity, transform.parent);
        }
    }

}

public enum DebrisTypes
{
    TV,
    OvenBathtub,
    Shelf,
}
