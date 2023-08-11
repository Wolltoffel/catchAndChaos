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
        Debug.Log("Parent-Idle");

        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);


        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushie(inputDevice);
        if (hasBeenThrown)
        {
            return new ParentLocked(parentData);
        }

        CheckDoorToggle(inputDevice);

        if (moveInput)
        {
            return new ParentMovement(parentData);
        }

        return this;
    }
}
