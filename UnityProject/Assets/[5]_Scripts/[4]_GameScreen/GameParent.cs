using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
    private ParentData parentData;

    private ParentBaseMovementState state;


    private void Awake()
    {
        parentData = GameData.GetData<ParentData>("Parent");

        //Spawn 3d model
        state = new ParentIdle(parentData);
    }

    private void FixedUpdate()
    {
        state = state.UpdateState();
    }
}
