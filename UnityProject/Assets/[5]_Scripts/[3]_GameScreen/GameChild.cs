using System.Collections;
using UnityEngine;

public class GameChild : MonoBehaviour
{
    private ChildState childState;

    private void Start()
    {
        ChildData temp = GameData.GetData<ChildData>("Child");
        temp.SlideCoolDown = 0;
        temp.IsSlideReady = true;

        childState = new ChildTutorial();
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
        bool interactableInRange = InteractableInRange(Characters.Child, out GameObject interactableObject, out string actionHint);

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
            ChildData childData = GameData.GetData<ChildData>("Child");
            if (currentObjectHash != objectHash || currentButtonPrompt == null || !childData.IsSlideReady)
            {
                currentObjectHash = objectHash;
                WorldSpaceUI.RemovePrompt(currentButtonPrompt);
                if (interactableObject != null)
                {
                    if (actionHint == "Vent")
                    {
                        if (childData.SlideCoolDown > 0)
                        {
                            Debug.Log(Mathf.Ceil(childData.SlideCoolDown));
                            string inputButton = $"{Mathf.Ceil(childData.SlideCoolDown)}Seconds";
                            WorldSpaceUI.ShowPrompt(GameData.GetData<PromptAssets>("PromptAssets").GetPromptAssetByName(inputButton), interactableObject.transform, "", out currentButtonPrompt);
                        }
                        else
                        {
                            WorldSpaceUI.ShowButtonPrompt(interactableObject.transform, inputButton, out currentButtonPrompt, actionHint);
                        }
                    }
                    else
                    {
                        WorldSpaceUI.ShowButtonPrompt(interactableObject.transform, inputButton, out currentButtonPrompt, actionHint);
                    }
                }
                if (lastVentInRange != null)
                    lastVentInRange.GetComponent<VentRollup>().CloseVent();
            }

            return HandleAction(interactableObject, actionHint);
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
        ChildData data = GameData.GetData<ChildData>("Child");

        lastVentInRange = interactableObject;

        //Toogle Vent
        VentRollup ventRollup = interactableObject.GetComponent<VentRollup>();
        if (ventRollup == null)
            ventRollup = interactableObject.AddComponent<VentRollup>();

        ventRollup.OpenVent();
            
        if (Input.GetButtonDown(inputButton))
        {
            if (data.SlideCoolDown <= 0)
            {
                //Remove Button Prompt
                WorldSpaceUI.RemovePrompt(currentButtonPrompt);
                currentButtonPrompt = null;

                //Handle Animations & Movement
                gameObject.GetComponent<MovementScript>().DoSlide(interactableObject, 0.4f / GameData.GetData<ChildData>("Child").tempSpeed);
                gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 4);

                //Play Sound
                SoundSystem.PlaySound("Slide3", 0.6f);

                return new Slide();
            }
            else
            {
                SoundSystem.PlaySound("NoSlide", 0.5f);
            }
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
            SoundSystem.PlaySound("eatingLolli");
            SoundSystem.PlayMusic("LollyMusic", 5, 0.5f);
        }     
    }

    protected ChildState Destroy(GameObject interactableObject)
    {
        if (Input.GetButtonDown(inputButton))
        {
            WorldSpaceUI.RemovePrompt(buttonPromptDestroy);
            WorldSpaceUI.RemovePrompt(currentButtonPrompt);
            currentButtonPrompt = null;
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

            timerLolly = 0;

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
        Vector2 inputVector = new Vector2(horizontal, vertical);

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
            lastVentInRange = null;
            ScreenSwitcher.OutsourceCoroutine(DecreaseSlideCoolDown(3));
            return new Idle();
        }
            
        return this;
    }

    private IEnumerator DecreaseSlideCoolDown(float slideCooldown)
    {
        ChildData data = GameData.GetData<ChildData>("Child");
        data.SlideCoolDown = slideCooldown;

        while (data.SlideCoolDown >= 0)
        {
            yield return null;
            data.SlideCoolDown -= Time.deltaTime;
        }

        data.SlideCoolDown = 0;
    }
}

class Destroy : ChildState
{
    float destroyTime;
    static float startDestroyTime;
    Destructable destructable;
    GameObject destroyPrompt;

    GameObject particleInstance;
    public Destroy (Destructable destructable)
    {
        this.destroyTime = destructable.destroyTimeLeft;
        startDestroyTime = Time.time;
        this.destructable = destructable;
        Camera.main.GetComponentInParent<CameraManager>().ApplyCameraShake(CameraManager.CameraShakeType.Chaos);
    }

    public override ChildState UpdateState()
    {
        if (Input.GetButtonUp(inputButton))
        {
            WorldSpaceUI.RemovePrompt(destroyPrompt);
            GameObject.Destroy (particleInstance);
            particleInstance =null;
            Camera.main.GetComponentInParent<CameraManager>().ApplyCameraShake(CameraManager.CameraShakeType.Off);
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
            destructable.transform,"DestroyMeter",out destroyPrompt);
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
            WorldSpaceUI.RemovePrompt(destroyPrompt);
            
            GameObject.Destroy (particleInstance);
            particleInstance =null;
            
            destructable.DestroyObject();
            destructable = null;
            Camera.main.GetComponentInParent<CameraManager>().ApplyCameraShake(CameraManager.CameraShakeType.Off);
            return new Idle();
        }
       
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex", 3);

        //Stunned
        ChildState stunned = Stunned();
        if (stunned != null)
        {   
            GameObject.Destroy (particleInstance);
            particleInstance =null;

            WorldSpaceUI.RemovePrompt(destroyPrompt);

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

class ChildTutorial : ChildState
{
    GameObject tutorialPrompt;
    public override ChildState UpdateState()
    {
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 0);
        
        if (tutorialPrompt==null)
        {
            WorldSpaceUI.ShowPrompt(GameData.GetData<PromptAssets>("PromptAssets").GetPromptAssetByName("TutorialChild"), 
                CharacterInstantiator.GetActiveCharacter(Characters.Child).transform,"",out tutorialPrompt);
        }
        
        //Go to Run
        float horizontal = Input.GetAxis(inputDevice + " Horizontal");
        float vertical = Input.GetAxis(inputDevice + " Vertical");

        if (horizontal != 0 || vertical != 0|| 
            Input.GetButton(inputDevice+"A")||Input.GetButton(inputDevice+"X")||
            Input.GetButton(inputDevice+"Y")||Input.GetButton(inputDevice+"B"))
        {
            WorldSpaceUI.RemovePrompt(tutorialPrompt);
            tutorialPrompt = null;
            return new Idle();
        }

        return this;

    }
}


