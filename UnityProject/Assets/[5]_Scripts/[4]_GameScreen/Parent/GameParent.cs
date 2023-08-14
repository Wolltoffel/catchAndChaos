using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
    private ParentData parentData;

    private ParentBaseMovementState state;


    private void Awake()
    {
        parentData = GameData.GetData<ParentData>("Parent");
        gameObject.layer = 7;

        //Spawn 3d model
        state = new ParentIdle(parentData);
    }

    private void Update()
    {
        state = state.UpdateState();
    }
}

abstract class ParentBaseMovementState : State
{
    protected ParentData parentData;

    public ParentBaseMovementState(ParentData data)
    {
        parentData = data;
    }

    public abstract ParentBaseMovementState UpdateState();

    protected void CheckDoorToggle(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Door", out interactable))
        {
            //show prompt

            if (Input.GetButtonDown($"{inputDevice}B"))
            {
                //DoAnimation
                //Sound

                DoorSwitch toggle = interactable.GetComponent<DoorSwitch>();
                toggle.Toggle();
            }
        }
        else
        {
            //hideprompt
        }
    }


    protected Plushie GetPlushieIfInRange(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            //show prompt

            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                //hideprompt

                Plushie plushie = interactable.GetComponent<Plushie>();

                return plushie;
            }
        }
        else
        {
            //hideprompt
        }
        return null;
    }

    protected bool CheckForPlushieAction(string inputDevice)
    {
        if (parentData.plushie == null)
        {
            parentData.plushie = GetPlushieIfInRange(inputDevice);
            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                if (parentData.plushie != null)
                {
                    parentData.plushie.AttachToTarget(gameObject.transform);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                return true;
            }
        }

        return false;
    }

    protected bool GetMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;
    }
}

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
            return new ParentThrow(parentData);
        }

        //CheckForDoors
        CheckDoorToggle(inputDevice);

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}A"))
        {
            MovementScript movement = gameObject.GetComponent<MovementScript>();
            movement.DoCatch();
            return new ParentCatch(parentData, movement);
        }

        if (moveInput)
        {
            return new ParentMovement(parentData);
        }

        return this;
    }
}

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
            return new ParentThrow(parentData);

        //MovePlayer
        movement.MovePlayer(xAxis, yAxis);
        if (!moveInput)
            return new ParentIdle(parentData);

        return this;
    }
}

class ParentCatch : ParentBaseMovementState
{
    private Coroutine catchCoroutine;
    private float time;
    private float catchDistance;
    private GameObject parent;
    private MovementScript movement;

    public ParentCatch(ParentData data, MovementScript movement) : base(data)
    {
        time = 0;
        parent = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        //movement = parent.GetComponent<MovementScript>();
        this.movement = movement;
        movement.DoCatch();
    }

    public override ParentBaseMovementState UpdateState()
    {
        if (movement.IsCoroutineDone)
        {
            return new ParentIdle(parentData);
        }

        return this;
    }
}

class ParentThrow : ParentBaseMovementState
{
    public ParentThrow(ParentData data) : base(data)
    {
        //Do Antimator

        Vector3 childTarget = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform.position;
        data.plushie.ThrowPlushie(childTarget);
    }

    public override ParentBaseMovementState UpdateState()
    {
        if (parentData.plushie.IsThrowDone)
        {
            parentData.plushie = null;
            return new ParentIdle(parentData);
        }
        return this;
    }

    private void ThrowPlushie()
    {

    }
}