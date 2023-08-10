using UnityEngine;

class ParentNoMovement : ParentBaseMovementState
{
    public ParentNoMovement(ParentData data) : base(data) {}

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;
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
        xAxis = Input.GetAxis(inputDevice + " Horizonal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;

    }
}
