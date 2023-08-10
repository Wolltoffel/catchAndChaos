using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
    private GameObject parent;
    private ParentData parentData;

    private ParentBaseMovementState movementState;


    private void Awake()
    {
        parentData = GameData.GetData<ParentData>("Parent");

        //Spawn 3d model
        movementState = new ParentIdle(parentData);
    }

    private void Update()
    {
        movementState = movementState.UpdateState();
    }
}
