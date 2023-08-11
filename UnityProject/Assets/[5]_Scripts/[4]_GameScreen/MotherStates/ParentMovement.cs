using UnityEngine.XR;
using UnityEngine;

class ParentMovement : ParentBaseMovementState
{
    private MovementScript movement;

    public ParentMovement(ParentData data) : base(data)
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);

        movement = gameObject.GetComponent<MovementScript>();
    }

    public override ParentBaseMovementState UpdateState()
    {
        Debug.Log("Parent-Movement");

        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);

        //CheckForDoorToggle
        CheckDoorToggle(inputDevice);

        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushie(inputDevice);
        if (hasBeenThrown)
            return new ParentLocked(parentData);

        //MovePlayer
        movement.MovePlayer(xAxis, yAxis);
        if (!moveInput)
            return new ParentIdle(parentData);

        return this;
    }
}
