using System.Collections;
using UnityEngine;

public class GameChild : MonoBehaviour
{
    private ChildState childState;

    private void Start()
    {
        childState = new Idle();
    }

    void  Update()
    {
        if (Time.timeScale!=0)
            childState = childState.UpdateState();
    }
}

abstract class ChildState : State
{   
    protected string inputDevice = GameData.GetData<ChildData>("Child").tempInputDevice;
    public abstract ChildState UpdateState();

    static GameObject buttonPrompt;

    public ChildState()
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
    }

    protected ChildState Slide()
    {
        if (InteractableInRange("Vent", out GameObject interactableObject) )
        {
            //Show ButtonPrompt  
            if (buttonPrompt==null)
                ButtonPromptManager.ShowButtonPrompt(interactableObject.transform, inputDevice+ "B",out buttonPrompt, "Vent");
            Debug.Log ("Showing Button Prompt");
            if (Input.GetButtonDown(inputDevice+"B"))
            {
                //Remove Button Prompt
                ButtonPromptManager.RemoveButtonPrompt(buttonPrompt);
                buttonPrompt = null;

                //Handle Animations & Movement
                Debug.Log ("Going through vent");
                gameObject.GetComponent<MovementScript>().DoSlide(interactableObject);
                gameObject.GetComponent<Animator>().SetInteger("ChildIndex",4);
                
                return new Slide();
            }    
        }
        else
        {
            ButtonPromptManager.RemoveButtonPrompt(buttonPrompt);
            buttonPrompt = null;
        }
        return null;
    }

    protected void LollyPickUp()
    {
         //Lolly PickUp
        /*if (InteractableInRange("Lolly", out GameObject interactableObject) )
        {   
            //Show Buttonprompt
            if (Input.GetButtonDown(inputDevice+"A"))
            {
                //Lollyspeed
            };    
        }*/
    }

    protected ChildState Stunned()
    {
        if (GameData.GetData<ChildData>("Child").stunned)
            return new Stunned();
        else
            return null;
    }

    
}

class Idle: ChildState
{
    public override ChildState UpdateState()
    {     
        //Debug.Log ("Idle");
        //Go to Run
        float horizontal = Input.GetAxis(inputDevice+" Horizontal");
        float vertical = Input.GetAxis(inputDevice+" Vertical");
        
        if (horizontal!=0 || vertical!=0)
        {
            return new Run();
        }

        LollyPickUp();

        //Slide
        ChildState slide = Slide();
        if (slide!=null)
            return slide;

        //Stunned
        ChildState stunned = Stunned();
        if (stunned!=null)
            return stunned;

        //Destroy Object
/*        if (InteractableInRange("Slidable", out GameObject interactableObject) )
        {   
            //Show Buttonprompt
            if (Input.GetButtonDown(inputDevice+"X"))
                return new Destroy();
        }*/

        gameObject.GetComponent<Animator>().SetInteger("ChildIndex",0);
        return this;
        
    }
}

class Run: ChildState
{ 
    public override ChildState UpdateState()
    {
        //LollyPickUp();

        float horizontal = Input.GetAxis(inputDevice+" Horizontal");
        float vertical = Input.GetAxis(inputDevice+" Vertical");
        Vector2 inputVector = new Vector2(horizontal,vertical).normalized;

        if (horizontal==0 && vertical==0)
        {
            return new Idle();
        }
        

        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        gameObject.GetComponent<MovementScript>().MovePlayer(inputVector.x, inputVector.y);
        gameObject.GetComponent<Animator>().SetInteger("ChildIndex",1);

        //Slide
        ChildState slide = Slide();
        if (slide!=null)
            return slide;

        
        //Stunned
        /*ChildState stunned = Stunned();
        if (stunned!=null)
            return stunned;*/

        return this;

    }
}

class Slide: ChildState
{
    private MovementScript movement;

    public Slide()
    {
        movement = CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<MovementScript>();
    }

    public override ChildState UpdateState()
    {
        if (movement.IsCoroutineDone)
            return new Idle();
       
        return this;
    }
}

class Destroy: ChildState
{
    public override ChildState UpdateState()
    {
        bool destroyOver= false; //ask from movement whether slide is over
        if (destroyOver)
            return new Idle();
    
        //Stunned
        ChildState stunned = Stunned();
        if (stunned!=null)
            return stunned;
        
        return this;
    } 
}

class Stunned: ChildState
{
    public override ChildState UpdateState()
    {
        float stunTime = GameData.GetData<ChildData>("ChildData").stunTime;
        float timer = Time.time;

        if (Time.time-timer >= stunTime)
            return new Idle();
        
        return this;
    }
}


