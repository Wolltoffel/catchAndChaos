using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParent : MonoBehaviour
{
    private ParentBaseState state;

    private void Awake()
    {
        gameObject.layer = 7;

        //Spawn 3d model
        state = new ParentIdle();
    }

    private void Update()
    {
        state = state.UpdateState();
    }
}

abstract class ParentBaseState : State
{
    protected ParentData parentData;
    static GameObject buttonPromptDoor,buttonPromptPlushiePickUp, buttonPromptPlushieThrow;
    static int currentDoorHash;

    public ParentBaseState()
    {
        parentData = GameData.GetData<ParentData>("Parent");
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
    }

    public abstract ParentBaseState UpdateState();

    protected void CheckDoorToggle(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Door", out interactable))
        {
            if (buttonPromptDoor == null)
            {
                WorldSpaceUI.ShowButtonPrompt(interactable.transform, inputDevice + "B", out buttonPromptDoor, "Door");
                currentDoorHash = interactable.GetHashCode();
            }
            else if (currentDoorHash != interactable.GetHashCode())
            {
                WorldSpaceUI.RemovePrompt(buttonPromptDoor);
                WorldSpaceUI.ShowButtonPrompt(interactable.transform, inputDevice + "B", out buttonPromptDoor, "Door");
                currentDoorHash = interactable.GetHashCode();
            }

            if (Input.GetButtonDown($"{inputDevice}B"))
            {
                gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);

                WorldSpaceUI.RemovePrompt(buttonPromptDoor);
                buttonPromptDoor = null;

                DoorSwitch toggle = interactable.GetComponent<DoorSwitch>();
                if (toggle == null)
                    toggle = interactable.GetComponentInParent<DoorSwitch>();
                if (toggle != null)
                {
                    toggle.Toggle();
                }
            }
        }
        else
        {
            WorldSpaceUI.RemovePrompt(buttonPromptDoor);
            buttonPromptDoor = null;
        }
    }


    protected Plushie GetPlushieIfInRange(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            if (buttonPromptPlushiePickUp == null)
            {
                WorldSpaceUI.ShowButtonPrompt(interactable.transform, inputDevice + "X", out buttonPromptPlushiePickUp, "PlushiePickUp");
            }

            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                WorldSpaceUI.RemovePrompt(buttonPromptPlushiePickUp);
                buttonPromptPlushiePickUp = null;

                Plushie plushie = interactable.GetComponent<Plushie>();

                return plushie;
            }
        }
        else
        {
            WorldSpaceUI.RemovePrompt(buttonPromptPlushiePickUp);
            buttonPromptPlushiePickUp = null;
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
                    gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);
                    parentData.plushie.AttachToTarget(gameObject.transform);
                }
            }
        }
        else
        {
            if (buttonPromptPlushieThrow == null)
            {
                Transform characterTransform = CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform;
                WorldSpaceUI.ShowButtonPrompt(characterTransform, inputDevice + "X", out buttonPromptPlushieThrow, "PlushieThrow");
            }

            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                WorldSpaceUI.RemovePrompt(buttonPromptPlushieThrow);
                buttonPromptPlushieThrow = null;
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

    protected bool IsGameOver(out ParentBaseState state)
    {
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");
        state = this;

        if (data.hasGameEnded)
        {
            if (data.hasChildWon)
                state = new ParentLose();
            else
                state = new ParentWin();
            return true;
        }

        return false;
    }
}

class ParentIdle : ParentBaseState
{
    public ParentIdle()
    {
        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 0);
    }

    public override ParentBaseState UpdateState()
    {
        if (IsGameOver(out ParentBaseState state))
            return state;

        string inputDevice = parentData.tempInputDevice;
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);


        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushieAction(inputDevice);
        if (hasBeenThrown)
        {
            return new ParentThrow();
        }

        //CheckForDoors
        CheckDoorToggle(inputDevice);

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}A"))
        {
            MovementScript movement = gameObject.GetComponent<MovementScript>();
            movement.DoCatch();
            return new ParentCatch();
        }

        if (moveInput)
        {
            return new ParentMovement();
        }

        return this;
    }
}

class ParentMovement : ParentBaseState
{
    private MovementScript movement;

    public ParentMovement()
    {
        movement = gameObject.GetComponent<MovementScript>();

        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 1);
    }

    public override ParentBaseState UpdateState()
    {
        if (IsGameOver(out ParentBaseState state))
            return state;

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
            return new ParentCatch();
        }

        //CheckForPlushie
        bool hasBeenThrown = CheckForPlushieAction(inputDevice);
        if (hasBeenThrown)
            return new ParentThrow();

        //MovePlayer
        movement.MovePlayer(xAxis, yAxis);
        if (!moveInput)
            return new ParentIdle();

        return this;
    }
}

class ParentCatch : ParentBaseState
{
    private Coroutine catchCoroutine;
    private float time;
    private float catchDistance;
    private MovementScript movement;

    public ParentCatch()
    {
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 4);
        this.movement = gameObject.GetComponent<MovementScript>();

        time = 0;
        movement.DoCatch();
    }

    public override ParentBaseState UpdateState()
    {
        if (movement.IsCoroutineDone)
        {
            return new ParentIdle();
        }

        Vector3 sphereCenter = gameObject.transform.position + Vector3.up + (gameObject.transform.forward)*0.5f;


        Collider[] colliders = Physics.OverlapSphere(sphereCenter, 1f);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == 8)
            {
                movement.StopCatch();

                GameScreenManager.EndGame(EndCondition.Catch);

                PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");
                data.hasGameEnded = true;

                return new ParentWin();
            }
        }

        return this;
    }
}

class ParentThrow : ParentBaseState
{
    public ParentThrow()
    {
        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 3);

        Vector3 childTarget = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform.position;
        parentData.plushie.ThrowPlushie(childTarget);
    }

    public override ParentBaseState UpdateState()
    {
        if (parentData.plushie.IsThrowDone)
        {
            parentData.plushie = null;
            return new ParentIdle();
        }
        return this;
    }
}

class ParentWin : ParentBaseState
{
    public ParentWin()
    {
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 7);
    }

    public override ParentBaseState UpdateState()
    {
        return this;
    }
}

class ParentLose : ParentBaseState
{
    public ParentLose()
    {
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 6);
    }

    public override ParentBaseState UpdateState()
    {
        return this;
    }
}

enum MotherInteractables
{
    Door,
    Plushie,

}