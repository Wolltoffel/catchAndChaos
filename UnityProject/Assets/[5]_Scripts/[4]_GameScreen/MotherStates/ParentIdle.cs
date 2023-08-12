using System;
using UnityEngine;
using UnityEngine.XR;

class ParentIdle : ParentBaseMovementState
{
    public ParentIdle(ParentData data) : base(data)
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
    }

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);


        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushieAction(inputDevice);
        if (hasBeenThrown)
        {
            return new ParentLocked(parentData);
        }

        //CheckForDoors
        CheckDoorToggle(inputDevice);

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}A"))
        {
            MovementScript movement = gameObject.GetComponent<MovementScript>();
            movement.DoCatch();
            return new ParentCatch(parentData,movement);
        }

        if (moveInput)
        {
            return new ParentMovement(parentData);
        }

        return this;
    }
}
