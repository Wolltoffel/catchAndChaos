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
            float randomRange = 0.03f;
            Vector3 randomPos = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
            GameObject obj = Instantiate(debris[i%debris.Length],transform.position + randomPos,Quaternion.identity, transform.parent);
            obj.transform.position = transform.position;
            ApplyRandomForce(obj);
        }
    }

    private void ApplyRandomForce(GameObject obj)
    {
        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

        rigidbody.AddExplosionForce(1, transform.position, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }

}

public enum DebrisTypes
{
    TV,
    OvenBathtub,
    Shelf,
}
