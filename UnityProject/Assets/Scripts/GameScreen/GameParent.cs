using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
  private GameObject parent;
  private ParentData parentData;

  private ParentBaseMovementState movementState;
  private ParentBaseObjectState objectState;


  private void Awake()
  {
    parentData = GameData.GetData<ParentData>("Parent");
    //update 3d model
    movementState = new ParentNoMovement(parentData);
    objectState = new ParentNoObject(parentData);
  }

  private void Update()
  {
    movementState = movementState.UpdateState();
    objectState = objectState.UpdateState();
  }
}
