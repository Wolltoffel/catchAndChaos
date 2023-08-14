using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectNavigation : MonoBehaviour
{
    [SerializeField]CharacterCustomSelectable [] selectables;
    [SerializeField] CharacterCustomSelectable startSelected;
    [SerializeField] Characters characters;
    [SerializeField]ConfirmSelectable confirmSelectable;
    CharacterCustomSelectable activeSelectable;
    string inputDevice;
    float lastInput;
    bool activeController;
    bool active;
    bool blockedInput;
    public bool isConfirmed;

    void Start()
    {
        activeSelectable = startSelected;
        activeSelectable.SelectCustomSelectable();
        
        for (int i = 0; i<selectables.Length; i++)
        {
            //Deactivate selectables
            selectables[i].gameObject.SetActive (false);
        }

        confirmSelectable.gameObject.SetActive (false);
    }

    void Update() 
    {
        isConfirmed = confirmSelectable.GetConfirmed(characters);

        WaitForControllerInput();
    }

    
    IEnumerator BlockInput()
    {
        blockedInput = true;

        float startTime = Time.unscaledTime;
        float waitTime = 0.2f;

        while (Time.unscaledTime-startTime<waitTime)
        {
           // If Input goes in a drastically different direction fast
            float verticalInput = Input.GetAxis (inputDevice+" Vertical");
           if (Mathf.Abs(lastInput-verticalInput)>0.6)
               break;
            yield return null;
        }
        blockedInput = false;
        StopAllCoroutines();
    }


    void WaitForControllerInput()
    {
        if (activeController && !blockedInput)
        {
            lastInput = Input.GetAxis(inputDevice+" Vertical");
            CharacterCustomSelectable nextCustomSelectable  = null;
            Selectable nextSelectable = null;

            if (lastInput<-0.5f)
                nextSelectable = activeSelectable.FindSelectableOnDown() as CharacterCustomSelectable; 

            else if (lastInput>0.5f)    
                nextSelectable  = activeSelectable.FindSelectableOnUp() as CharacterCustomSelectable;

            if (nextSelectable is CharacterCustomSelectable)
                nextCustomSelectable = nextSelectable as CharacterCustomSelectable;
            else if (nextSelectable is not CharacterCustomSelectable && nextSelectable!=null)
                throw new System.Exception ("No custom CharactterSelectable input");

            if (nextCustomSelectable!=null)
            {    
                    activeSelectable.DeselectCustomSelectable();
                    activeSelectable = nextCustomSelectable;
                    activeSelectable.SelectCustomSelectable();
                    StartCoroutine (BlockInput());
            }   
        
        }
    }

    bool CheckForController()
    {
        for (int i = 1; i<=4;i++)
        {
            if (inputDevice == "J"+i)
                return true;
        }
        return false;

    }

    public void ActivateCharacterSelection()
    {
        active = true;

        for (int i = 0; i<selectables.Length; i++)
        {
            selectables[i].gameObject.SetActive(true);
            selectables[i].interactable = true;
        } 

        confirmSelectable.gameObject.SetActive (true);  
        confirmSelectable.interactable = true;

        //Find out input device
        string key = "";
        if (characters == Characters.Child)
            key = "Child";
        else
            key = "Parent";

        
        inputDevice = GameData.GetData<PlayerData>(key).tempInputDevice;
        activeController = CheckForController();

        for (int i = 0; i<selectables.Length; i++)
        {
            //Set Data to selectables
            selectables[i].SetData(inputDevice, activeController, characters);
        }

       confirmSelectable.SetData(characters,inputDevice, activeController);
    }

}
