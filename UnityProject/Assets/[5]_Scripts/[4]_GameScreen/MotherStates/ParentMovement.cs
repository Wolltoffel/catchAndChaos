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
        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);

        //CheckForDoorToggle
        CheckDoorToggle(inputDevice);

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}A"))
        {
            movement.DoCatch();
            return new ParentCatch(parentData, movement);
        }

        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushieAction(inputDevice);
        if (hasBeenThrown)
            return new ParentLocked(parentData);

        //MovePlayer
        movement.MovePlayer(xAxis, yAxis);
        if (!moveInput)
            return new ParentIdle(parentData);

        return this;
    }
}
