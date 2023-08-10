using UnityEngine;
using UnityEngine.XR;

class ParentIdle : ParentBaseMovementState
{
    public ParentIdle(ParentData data, ParentBaseObjectState objectState) : base(data, objectState)
    {
        gameObject = data.gameObject;
    }

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = CheckMovement(inputDevice, out xAxis, out yAxis);

        //UpdateObjectState
        objectState = objectState.UpdateState();

        CheckDoorToggle(inputDevice);

        if (moveInput)
        {
            return new ParentMovement(parentData,objectState);
        }

        return this;
    }

    public bool CheckMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;

    }
}
