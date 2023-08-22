using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float destroyTimeLeft;


    private void Awake()
    {
        destroyTimeLeft = GameData.GetData<ChildData>("Child").timeToDestroy;
    }

}
