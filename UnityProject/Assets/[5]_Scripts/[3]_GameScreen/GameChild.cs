using System.Collections;
using UnityEngine;

public class GameChild : MonoBehaviour
{
    private ChildState childState;

    private void Start()
    {
        childState = new Idle();
    }

    void Update()
    {
        if (Time.timeScale != 0)
            childState = childState.UpdateState();
    }
}

abstract class ChildState : State
{
    protected string inputDevice = GameData.GetData<ChildData>("Child").tempInputDevice;
    public abstract ChildState UpdateState();

    static GameObject buttonPromptSlide, buttonPromptLolly, buttonPromptDestroy;
    static GameObject currentButtonPrompt;
    protected static GameObject lastVentInRange;
    static float timerLolly;
    static bool lollyPickedUp;
    string lastAction;

    protected string inputButton { get => inputDevice + "A"; }

    public ChildState()
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        GameData.GetData<ChildData>("Child").tempSpeed = GameData.GetData<ChildData>("Child").defaultSpeed;
    }

    protected ChildState CheckAction()
    {
        bool interactableInRange = InteractableInRange(Characters.Child, out GameObject interactableObject, out string action);

        //HandleSpeedBoost
        if (lollyPickedUp)
        {
            ChildData childData = GameData.GetData<ChildData>("Child");
            timerLolly -= Time.deltaTime;

            if (timerLolly > 0)
            {
                childData.tempSpeed = childData.lollySpeed;
            }

            else
            {
                childData.tempSpeed = childData.defaultSpeed;
                lollyPickedUp = false;
            }
        }

        if (interactableInRange)
        {
            //Check if its the same object as so far
            int objectHash = interactableObject.GetHashCode();
            if (currentObjectHash != objectHash || currentButtonPrompt == null)
            {
                currentObjectHash = objectHash;
                WorldSpaceUI.RemovePrompt(currentButtonPrompt);
                WorldSpaceUI.ShowButtonPrompt(interactableObject.transform, inputButton, out currentButtonPrompt, action);

                if (lastVentInRange != null)
                    lastVentInRange.GetComponent<VentRollup>().CloseVent();
            }

            return HandleAction(interactableObject, action);
        }


        //RemovePrompt
        if (currentButtonPrompt != null)
        {
            WorldSpaceUI.RemovePrompt(currentButtonPrompt);
            currentButtonPrompt = null;
        }

        //VentRollup
        if (lastVentInRange != null)
            lastVentInRange.GetComponent<VentRollup>().CloseVent();

        return null;
    }

    private ChildState HandleAction(GameObject interactableObject, string action)
    {
        ChildState returnState = null;

        switch (action)
        {
            case "Vent":
                returnState = Slide(interactableObject);
                break;
            case "Chaos":
                returnState = Destroy(interactableObject);
                break;
            case "Lolly":
                LollyPickUp(true, interactableObject);
                break;
            default:
                Debug.Log($"ERROR! NO ACTION FOUND!!! ACTION: {action}");
                break;
        }

        return returnState;
    }

    protected ChildState Slide(GameObject interactableObject)
    {
        lastVentInRange = interactableObject;

        //Toogle Vent
        VentRollup ventRollup = interactableObject.GetComponent<VentRollup>();
        if (ventRollup == null)
            ventRollup = interactableObject.AddComponent<VentRollup>();

        ventRollup.OpenVent();
            
        if (Input.GetButtonDown(inputButton))
        {
            //Remove Button Prompt
            WorldSpaceUI.RemovePrompt(currentButtonPrompt);
            currentButtonPrompt = null;

            //Handle Animations & Movement
            Debug.Log("Going through vent");
            gameObject.GetComponent<MovementScript>().DoSlide(interactableObject);
            gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 4);

            return new Slide();
        }
        return null;
    }

    protected void LollyPickUp(bool isLolly,GameObject interactableObject)
    {
        if (Input.GetButtonDown(inputButton))
        {
            //Lollyspeed
            lollyPickedUp = true;
            WorldSpaceUI.RemovePrompt(buttonPromptLolly);
            buttonPromptLolly = null;
            GameData.GetData<InteractableContainer>("InteractableContainer").RemoveObjectFromCategory("Lolly", interactableObject);
            GameObject.Destroy(interactableObject);
            timerLolly = GameData.GetData<ChildData>("Child").lollyDuration;
        }     
    }

    protected ChildState Destroy(GameObject interactableObject)
    {
        if (Input.GetButtonDown(inputButton))
        {
            WorldSpaceUI.RemovePrompt(buttonPromptDestroy);
            buttonPromptDestroy = null;
            return new Destroy(interactableObject.GetComponent<Destructable>());
        }
        return null;
    }

    protected ChildState Stunned()
    {
        if (GameData.GetData<ChildData>("Child").stunned)
            return new Stunned();
        else
            return null;
    }

    protected bool IsGameOver(out ChildState state)
    {
        PlayTimeData data = GameData.GetData<PlayTimeData>("PlayTimeData");
        state = this;

        if (data.hasGameEnded)
        {
            if (data.hasChildWon)
                state = new Win();
            else
                state = new Lose();
            return true;
        }

        return false;
    }
}

class Win : ChildState
{
    public Win()
    {
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 7);
    }

    public override ChildState UpdateState()
    {
        return this;
    }
}

class Lose : ChildState
{
    public Lose()
    {
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 6);
    }

    public override ChildState UpdateState()
    {
        return this;
    }
}

class Idle : ChildState
{
    public override ChildState UpdateState()
    {
        if (IsGameOver(out ChildState state))
            return state;

        //Check Stun
        ChildState stunned = Stunned();
        if (stunned != null)
            return stunned;

        //Check Action
        ChildState action = CheckAction();
        if (action != null)
            return action;

        //Go to Run
        float horizontal = Input.GetAxis(inputDevice + " Horizontal");
        float vertical = Input.GetAxis(inputDevice + " Vertical");

        if (horizontal != 0 || vertical != 0)
            return new Run();

        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 0);
        return this;

    }
}

class Run : ChildState
{
    public override ChildState UpdateState()
    {
        if (IsGameOver(out ChildState state))
            return state;

        //Check Stun
        ChildState stunned = Stunned();
        if (stunned != null)
            return stunned;

        //Check Action
        ChildState action = CheckAction();
        if (action != null)
            return action;

        float horizontal = Input.GetAxis(inputDevice + " Horizontal");
        float vertical = Input.GetAxis(inputDevice + " Vertical");
        Vector2 inputVector = new Vector2(horizontal, vertical).normalized;

        if (horizontal == 0 && vertical == 0)
        {
            return new Idle();
        }


        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        var t = gameObject.GetComponent<MovementScript>().MovePlayer(inputVector.x, inputVector.y, GameData.GetData<ChildData>("Child").tempSpeed);

        if (t == Vector2.zero)
            gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 0);
        else
            gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 1);

        return this;

    }
}

class Slide : ChildState
{
    private MovementScript movement;

    public Slide()
    {
        movement = CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<MovementScript>();
    }

    public override ChildState UpdateState()
    {
        if (movement.IsCoroutineDone)
        {
            VentRollup ventRollUp = lastVentInRange.GetComponent<VentRollup>();
            ventRollUp.CloseVent();
            GameObject.Destroy(ventRollUp);
            lastVentInRange = null;
            return new Idle();
        }
            
        return this;
    }
}

class Destroy : ChildState
{
    float destroyTime;
    static float startDestroyTime;
    Destructable destructable;

    GameObject destroyPrompt;
    GameObject promptHolder;

    GameObject particleInstance;
    public Destroy (Destructable destructable)
    {
        this.destroyTime = destructable.destroyTimeLeft;
        startDestroyTime = Time.time;
        this.destructable = destructable;
    }

    public override ChildState UpdateState()
    {
        if (Input.GetButtonUp(inputButton))
        {
            WorldSpaceUI.RemovePrompt(promptHolder);
            GameObject.Destroy (particleInstance);
            particleInstance =null;
            return new Idle();
        }

        destroyTime -= Time.deltaTime;

        if (particleInstance==null)
        {
            //Spawn Particles on destructable
            Vector3 position = destructable.gameObject.transform.position;
            GameObject particles = GameData.GetData<ChildData>("Child").destroyParticles;
            particleInstance = GameObject.Instantiate(particles);
            particleInstance.transform.position = position;
            particleInstance.transform.localScale = new Vector3(2,2,2);
            Debug.Log (particleInstance.name);
            Debug.Log ("SpawnedParticles");
        }

        //Show Prompt
        if (destroyPrompt==null)
            WorldSpaceUI.ShowPrompt(GameData.GetData<PromptAssets>("PromptAssets").GetPromptAssetByName("DestroyMeter"),
            destructable.transform,"DestroyMeter",out promptHolder,out destroyPrompt);
        else
        {
            DestroyMeter destroyMeter = destroyPrompt.GetComponent <DestroyMeter>();
            if (destroyMeter!=null)
            {
                destroyMeter.UpdateProgress(destroyTime,GameData.GetData<ChildData>("Child").timeToDestroy);
                destructable.destroyTimeLeft = destroyTime;
            }
                
        }      
        
        if (destroyTime <= 0)
        {
            WorldSpaceUI.RemovePrompt(promptHolder);
            
            GameObject.Destroy (particleInstance);
            particleInstance =null;
            
            destructable.DestroyObject();
            destructable = null;
            return new Idle();
        }
       
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex", 3);

        //Stunned
        ChildState stunned = Stunned();
        if (stunned != null)
        {   
            GameObject.Destroy (particleInstance);
            particleInstance =null;

            WorldSpaceUI.RemovePrompt(promptHolder);

            return stunned;
        }

        return this;
    }
}

class Stunned : ChildState
{
    ChildData data;
    float timer;

    public Stunned()
    {
        data = GameData.GetData<ChildData>("Child");
        timer = data.stunTime;
    }
    public override ChildState UpdateState()
    {
        timer-= Time.deltaTime;

        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 2);

        if (timer <= 0)
        {
            data.stunned = false;
            return new Idle();
        }

        return this;
    }
}


