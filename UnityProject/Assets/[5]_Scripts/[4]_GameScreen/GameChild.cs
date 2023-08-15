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

    public ChildState()
    {
        gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
    }

    protected ChildState Slide()
    {
        //Slide
       /* if (InteractableInRange("Wall", out GameObject interactableObject) )
        {   
            //Show Buttonprompt
            if (Input.GetButtonDown(inputDevice+"B"))
            {
                //Lollyspeed
                return new Slide();
            }    
        }*/
        return null;
    }

    protected void LollyPickUp()
    {
         //Lolly PickUp
        /*if (InteractableInRange("Lolly", out GameObject interactableObject) )
        {   
            //Show Buttonprompt
            if (Input.GetButtonDown(inputDevice+"B"))
            {
                //Lollyspeed
            };    
        }*/
    }

    
}

class Idle: ChildState
{
    public override ChildState UpdateState()
    {
        
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

        //Destroy Object
/*        if (InteractableInRange("Slidable", out GameObject interactableObject) )
        {   
            //Show Buttonprompt
            if (Input.GetButtonDown(inputDevice+"X"))
                return new Destroy();
        }*/

        return this;
        
    }
}

class Run: ChildState
{
    public override ChildState UpdateState()
    {
        LollyPickUp();

        //Turn to idle
        float horizontal = Input.GetAxis(inputDevice+" Horizontal");
        float vertical = Input.GetAxis(inputDevice+" Vertical");

        GameObject gameObject = CharacterInstantiator.GetActiveCharacter(Characters.Child);
        gameObject.GetComponent<MovementScript>().MovePlayer(horizontal, vertical);


        if (horizontal!=0 || vertical!=0)
        {
            return new Idle();
        }
        
        //Slide
        ChildState slide = Slide();
        if (slide!=null)
            return slide;

        return this;

    }
}

class Slide: ChildState
{
    public override ChildState UpdateState()
    {
        bool slideOver= false; //ask from movement whether slide is over
        if (slideOver)
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
        
        return this;
    }
}

