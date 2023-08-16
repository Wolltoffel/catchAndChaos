using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
    private ParentData parentData;

    private ParentBaseMovementState state;

   /* private void OnDrawGizmos()
    {
        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position + Vector3.up + (gameObject.transform.forward), 1);
    }*/

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
                //Do Antimator
                gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);

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
                    //Do Antimator
                    gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);

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
        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 0);
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

        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 1);
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
    private MovementScript movement;

    public ParentCatch(ParentData data, MovementScript movement) : base(data)
    {
        time = 0;
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        //movement = parent.GetComponent<MovementScript>();
        this.movement = movement;
        movement.DoCatch();

        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 4);
    }

    public override ParentBaseMovementState UpdateState()
    {
        if (movement.IsCoroutineDone)
        {
            return new ParentIdle(parentData);
        }

        Vector3 sphereCenter = gameObject.transform.position + Vector3.up + (gameObject.transform.forward);

        if (Physics.CheckSphere(sphereCenter, 0.5f, 8))
        {
            Debug.Log("Order has been restored");
            GameScreenManager.Endgame(EndCondition.Catch);
        }

        return this;
    }
}

class ParentThrow : ParentBaseMovementState
{
    public ParentThrow(ParentData data) : base(data)
    {
        //Do Antimator
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 3);

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
}