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

    static GameObject buttonPromptSlide, buttonPromptLolly;
    protected static GameObject lastVentInRange;
    static float startTimeLolly;
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
            startTimeLolly = Time.time;
            ChildData childData = GameData.GetData<ChildData>("Child");

            if (Time.time - startTimeLolly <= childData.lollyDuration)
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


        //Stunned
        /*ChildState stunned = Stunned();
        if (stunned!=null)
            return stunned;*/

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
    public override ChildState UpdateState()
    {
        if (Input.GetButtonUp(inputDevice+"X"))
            return new Idle();

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

        if (Time.time - timer >= stunTime)
            return new Idle();

        return this;
    }
}


