using UnityEngine.XR;
using UnityEngine;

class ParentMovement : ParentBaseMovementState
{
    private MovementScript movement;

    public ParentMovement(ParentData data) : base(data)
    {
        gameObject = data.gameObject;

        movement = gameObject.GetComponent<MovementScript>();
    }

    public override ParentBaseMovementState UpdateState()
    {
        string inputDevice = parentData.tempInputDevice;

        //remove
        inputDevice = "K1";
        //remove

        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);

        //Customcode
        CheckPlushie(inputDevice);
        CheckDoorToggle(inputDevice);
        movement.MovePlayer(xAxis, yAxis);

        if (!moveInput)
        {
            return new ParentIdle(parentData);
        }

        return this;
    }

    public bool GetMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;

    }
}
