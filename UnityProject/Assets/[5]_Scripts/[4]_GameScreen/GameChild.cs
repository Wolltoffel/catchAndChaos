using System.Collections;
using UnityEngine;

public class GameChild : MonoBehaviour
{
  void  Update()
  {
    
  }
}

abstract class ChildState : State
{   
    protected string inputDevice = GameData.GetData<ChildData>("Child").tempInputDevice;
    public abstract ChildState UpdateState();

    public ChildState()
    {
        gameObject = GameData.GetData<ChildData>("Child").gameObject;
    }

    
}

class Idle: ChildState
{
    public override ChildState UpdateState()
    {
        if (Input.GetButtonDown(inputDevice+"A"))
        {

        }

        float horizontal = Input.GetAxis(inputDevice+" Horizontal");
        float vertical = Input.GetAxis(inputDevice+" Vertical");

        if (horizontal>0 || vertical>0)
        {
            return new Run();
        }

        else if (InteractableInRange("Lolly", out GameObject interactableObject) 
        && Input.GetButtonDown(inputDevice+"B"))
        {
            return new Lolly();
        }

        return this;

    }
}

class Run: ChildState
{
    public override ChildState UpdateState(){return this;}
}

class Lolly: ChildState
{
    public override ChildState UpdateState(){return this;}
}
