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
    protected static GameObject lastVentInRange;
    static float timerLolly;
    static bool lollyPickedUp;

    public ChildState()
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        GameData.GetData<ChildData>("Child").tempSpeed = GameData.GetData<ChildData>("Child").defaultSpeed;
    }

    protected ChildState Slide()
    {
        bool interactableInRange = InteractableInRange("Vent", out GameObject interactableObject);
        if (interactableInRange)
        {
            lastVentInRange = interactableObject;

            //Show ButtonPrompt  
            if (buttonPromptSlide == null)
                ButtonPromptManager.ShowButtonPrompt(interactableObject.transform, inputDevice + "B", out buttonPromptSlide, "Vent");
            //Toogle Vent
            VentRollup ventRollup = interactableObject.GetComponent<VentRollup>();
            if (ventRollup == null)
                ventRollup = interactableObject.AddComponent<VentRollup>();

            ventRollup.OpenVent();
            
             if (Input.GetButtonDown(inputDevice + "B"))
             {
                //Remove Button Prompt
                ButtonPromptManager.RemoveButtonPrompt(buttonPromptSlide);
                buttonPromptSlide = null;

                //Handle Animations & Movement
                Debug.Log("Going through vent");
                gameObject.GetComponent<MovementScript>().DoSlide(interactableObject);
                gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 4);

                return new Slide();
               }
        }
        else
        {
            if (lastVentInRange!=null)
            {
                lastVentInRange.GetComponent<VentRollup>().CloseVent();
            }
            
            ButtonPromptManager.RemoveButtonPrompt(buttonPromptSlide);
            buttonPromptSlide = null;
        }
        return null;
    }

    protected void LollyPickUp()
    {
        //Lolly PickUp
        if (InteractableInRange("Lolly", out GameObject interactableObject) )
        {
            if (buttonPromptLolly == null)
            {
                ButtonPromptManager.ShowButtonPrompt(interactableObject.transform, inputDevice + "A", out buttonPromptLolly, "Lolly");
            }
            
            if (Input.GetButtonDown(inputDevice+"A"))
            {
                //Lollyspeed
                lollyPickedUp = true;
                ButtonPromptManager.RemoveButtonPrompt(buttonPromptLolly);
                buttonPromptLolly = null;
                GameData.GetData<InteractableContainer>("InteractableContainer").RemoveObjectFromCategory("Lolly", interactableObject);
                GameObject.Destroy(interactableObject);
                timerLolly = GameData.GetData<ChildData>("Child").lollyDuration;

            };     
        }
        else
        {
            ButtonPromptManager.RemoveButtonPrompt(buttonPromptLolly);
            buttonPromptLolly = null;
        }

        //Apply speed boost
        if (lollyPickedUp)
        {
            ChildData childData = GameData.GetData<ChildData>("Child");
            timerLolly -= Time.deltaTime;

            if (timerLolly>0)
            {
                childData.tempSpeed = childData.lollySpeed;
            }
                
            else
            {
                childData.tempSpeed = childData.defaultSpeed;
                lollyPickedUp = false;
            }        
        }
    }

    protected ChildState Stunned()
    {
        if (GameData.GetData<ChildData>("Child").stunned)
            return new Stunned();
        else
            return null;
    }

    protected ChildState Destroy()
    {
        if (InteractableInRange("Chaos", out GameObject interactableObject))
        {
            if (buttonPromptDestroy == null)
            {
                ButtonPromptManager.ShowButtonPrompt(interactableObject.transform, inputDevice + "X", out buttonPromptDestroy, "Chaos");
            }

            if (Input.GetButtonDown(inputDevice + "X"))
            {
                ButtonPromptManager.RemoveButtonPrompt(buttonPromptDestroy);
                buttonPromptDestroy = null;
                return new Destroy(interactableObject.GetComponent<Destructable>());
            }
        }
        else
        {
            ButtonPromptManager.RemoveButtonPrompt(buttonPromptDestroy);
            buttonPromptDestroy = null;
        }
        return null;
    }


}

class Idle : ChildState
{
    public override ChildState UpdateState()
    {
        //Debug.Log ("Idle");
        //Go to Run
        float horizontal = Input.GetAxis(inputDevice + " Horizontal");
        float vertical = Input.GetAxis(inputDevice + " Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            return new Run();
        }

        LollyPickUp();

        //Slide
        ChildState slide = Slide();
        if (slide != null)
            return slide;

        //Stunned
        ChildState stunned = Stunned();
        if (stunned != null)
            return stunned;

        //Destroy
        ChildState destroy = Destroy();
        if (destroy != null)
            return destroy;

        //Destroy Object
        /*        if (InteractableInRange("Slidable", out GameObject interactableObject) )
                {   
                    //Show Buttonprompt
                    if (Input.GetButtonDown(inputDevice+"X"))
                        return new Destroy();
                }*/

        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 0);
        return this;

    }
}

class Run : ChildState
{
    public override ChildState UpdateState()
    {
        LollyPickUp();

        float horizontal = Input.GetAxis(inputDevice + " Horizontal");
        float vertical = Input.GetAxis(inputDevice + " Vertical");
        Vector2 inputVector = new Vector2(horizontal, vertical).normalized;

        if (horizontal == 0 && vertical == 0)
        {
            return new Idle();
        }


        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        gameObject.GetComponent<MovementScript>().MovePlayer(inputVector.x, inputVector.y, GameData.GetData<ChildData>("Child").tempSpeed);
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex", 1);

        //Slide
        ChildState slide = Slide();
        if (slide != null)
            return slide;


        //Destroy
        ChildState destroy = Destroy();
        if (destroy != null)
            return destroy;

        //Stunned
        ChildState stunned = Stunned();
        if (stunned!=null)
            return stunned;

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
    public Destroy (Destructable destructable)
    {
        this.destroyTime = destructable.destroyTimeLeft;
        startDestroyTime = Time.time;
        this.destructable = destructable;
    }
    public override ChildState UpdateState()
    {
        if (Input.GetButtonUp(inputDevice+"X"))
            return new Idle();

        destroyTime -= Time.deltaTime;
        
        if (destroyTime <= 0)
        {
            GameData.GetData<InteractableContainer>("InteractableContainer").RemoveObjectFromCategory("Chaos", destructable.gameObject);
            GameObject.Destroy(destructable.gameObject);
            destructable = null;
            GameData.GetData<ChaosData>("ChaosData").ModifyChaos(GameData.GetData<ChildData>("Child").chaosScorePerChaosObject);
            return new Idle();
        }
        //GameData.GetData<InteractableContainer>("InteractableContainer").RemoveObjectFromCategory("Destroy", interactableObject);
       
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex", 3);

        //Stunned
        ChildState stunned = Stunned();
        if (stunned != null)
            return stunned;

        return this;
    }
}

class Stunned : ChildState
{
    public override ChildState UpdateState()
    {
        float stunTime = GameData.GetData<ChildData>("ChildData").stunTime;
        float timer = Time.time;

        if (Time.time - timer <= stunTime)
            return new Idle();

        return this;
    }
}


