using UnityEngine;

class ParentIdle : ParentBaseMovementState
{
    public ParentIdle(ParentData data) : base(data) {}

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;

        //remove
        inputDevice = "K1";
        //remove

        float xAxis;
        float yAxis;
        bool moveInput = CheckMovement(inputDevice, out xAxis, out yAxis);

        if (moveInput)
        {
            return new ParentMovement(parentData);
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