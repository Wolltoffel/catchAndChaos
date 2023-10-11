using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.EventSystems.PointerEventData;

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
    protected string inputButton;
    protected static bool hasPlushie;
    private static GameObject currentButtonPrompt;

    public ParentBaseState()
    {
        parentData = GameData.GetData<ParentData>("Parent");
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        inputButton = parentData.tempInputDevice + "A";
    }

    public abstract ParentBaseState UpdateState();

    protected ParentBaseState CheckAction()
    {
        if (parentData.plushie == null)
        {
            bool interactableInRange = InteractableInRange(Characters.Parent, out GameObject interactableObject, out string action);

            if (interactableInRange)
            {
                //Check if its the same object as so far
                int objectHash = interactableObject.GetHashCode();
                if (currentObjectHash != objectHash || currentButtonPrompt == null)
                {
                    currentObjectHash = objectHash;
                    if (currentButtonPrompt != null)
                    {
                        WorldSpaceUI.RemovePrompt(currentButtonPrompt);
                        Debug.Log("REMOVED");
                    }

                    string hint = action;
                    if (hint == "Plushie")
                        hint = "PlushiePickUp";

                    Debug.Log(action);
                    WorldSpaceUI.ShowButtonPrompt(interactableObject.transform, inputButton, out currentButtonPrompt, hint);
                }

                if (Input.GetButtonDown(inputButton))
                {
                    return HandleAction(interactableObject, action);
                }

                return null;
            }

            //RemovePrompt
            if (currentButtonPrompt != null)
            {
                WorldSpaceUI.RemovePrompt(currentButtonPrompt);
                currentButtonPrompt = null;
            }

            return null;
        }
        else
        {
            if (Input.GetButtonDown(inputButton))
                return HandleAction(null, "Plushie");
            return null;
        }        
    }

    private ParentBaseState HandleAction(GameObject interactableObject, string action)
    {
        ParentBaseState returnState = null;
        switch (action)
        {
            case "Door":
                returnState = ToggleDoor(interactableObject);
                break;
            case "Plushie":
                returnState = TogglePlushie(interactableObject);
                break;
            default:
                break;
        }

        return returnState;
    }

    protected ParentBaseState ToggleDoor(GameObject interactable)
    {
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);

        DoorSwitch toggle = interactable.GetComponent<DoorSwitch>();
        if (toggle == null)
            toggle = interactable.GetComponentInParent<DoorSwitch>();
        if (toggle != null)
        {
            toggle.Toggle();
        }

        return null;
    }

    protected ParentBaseState TogglePlushie(GameObject interactable)
    {
        if (parentData.plushie == null)
        {
            WorldSpaceUI.RemovePrompt(currentButtonPrompt);
            WorldSpaceUI.ShowButtonPrompt(gameObject.transform, inputButton, out currentButtonPrompt, "PlushieThrow");

            parentData.plushie = interactable.GetComponent<Plushie>();

            gameObject.GetComponent<Animator>().SetInteger("MomIndex", 5);
            Transform handTransform = FindDeepChild(gameObject.transform, "Right wrist");

            parentData.plushie.AttachToTarget(handTransform);
            return null;
        }
        else
        {
            WorldSpaceUI.RemovePrompt(currentButtonPrompt);
            currentButtonPrompt = null;
            return new ParentThrow();
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
        {
            return result;
        }

        foreach (Transform child in parent)
        {
            result = FindDeepChild(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
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

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}B"))
        {
            MovementScript movement = gameObject.GetComponent<MovementScript>();
            movement.DoCatch();
            return new ParentCatch();
        }

        //CheckForAction
        ParentBaseState action = CheckAction();
        if (action != null)
        {
            return action;
        }

        //CheckForMove
        float xAxis;
        float yAxis;
        bool moveInput = GetMovement(inputDevice, out xAxis, out yAxis);
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

        //CheckForCatch
        if (Input.GetButtonDown($"{inputDevice}B"))
        {
            movement.DoCatch();
            return new ParentCatch();
        }

        //CheckForAction
        ParentBaseState action = CheckAction();
        if (action != null)
        {
            return action;
        }

        //MovePlayer
        Vector2 move = movement.MovePlayer(xAxis, yAxis);
        if (!moveInput)
            return new ParentIdle();

        if (move == Vector2.zero)
        {
            gameObject.GetComponent<Animator>().SetInteger("MomIndex", 0);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetInteger("MomIndex", 1);
        }

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
    private float timeElapsed = 0;
    private bool hasThrown = false;
    private Transform target;

    public ParentThrow()
    {
        //Do Antimator
        gameObject.GetComponent<Animator>().SetInteger("MomIndex", 3);

        timeElapsed = 0;
        hasThrown = false;

        Transform childTarget = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform;

        Vector3 targetDir = (childTarget.position - gameObject.transform.position).normalized;

        LayerMask obstacleLayer;
        obstacleLayer = ~6;
        target = Vector3.Dot(targetDir, gameObject.transform.forward) > 0.2f ? !Physics.Linecast(gameObject.transform.position, childTarget.position, out RaycastHit hit, obstacleLayer) ? target : null : null;
    }

    public override ParentBaseState UpdateState()
    {
        if (!hasThrown)
        {
            Transform childTarget = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform;
            Vector3 targetDir = (childTarget.position - gameObject.transform.position).normalized;

            if (timeElapsed < 0.61f)
            {
                LayerMask obstacleLayer;
                obstacleLayer = ~6;
                target = Vector3.Dot(targetDir, gameObject.transform.forward) > 0.2f ? !Physics.Linecast(gameObject.transform.position, childTarget.position, out RaycastHit hit, obstacleLayer) ? childTarget : null : null;

                if (target != null)
                { 
                    gameObject.transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(targetDir),gameObject.transform.rotation, 0.8f);
                }
                timeElapsed += Time.deltaTime;
                return this;
            }

            targetDir = target == null ? gameObject.transform.forward : (childTarget.position - gameObject.transform.position).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(targetDir);
            Vector3 throwDir = target == null? targetDir : ((childTarget.position + Vector3.up) - parentData.plushie.transform.position).normalized;

            parentData.plushie.ThrowPlushie(throwDir);
            hasThrown = true;

            return this;
        }

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