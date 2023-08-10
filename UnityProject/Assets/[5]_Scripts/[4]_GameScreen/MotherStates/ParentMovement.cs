using UnityEngine.XR;
using UnityEngine;

class ParentMovement : ParentBaseMovementState
{
    private MovementScript movement;

    public ParentMovement(ParentData data, ParentBaseObjectState objectState) : base(data, objectState)
    {
        gameObject = data.gameObject;

        movement = gameObject.GetComponent<MovementScript>();
    }

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);

        //CheckForDoorToggle
        CheckDoorToggle(inputDevice);

        //MovePlayer
        movement.MovePlayer(xAxis, yAxis);

        //UpdateObjectState
        bool hasBeenThrown;
        objectState = objectState.UpdateState(out hasBeenThrown);
        if (hasBeenThrown)
            return new ParentIdle(parentData, objectState);

        if (!moveInput)
            return new ParentIdle(parentData,objectState);

        return this;
    }

    public bool GetMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;

    }
}
