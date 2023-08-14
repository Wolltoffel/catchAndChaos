using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectNavigation : MonoBehaviour
{
    [SerializeField]CharacterCustomSelectable [] selectables;
    [SerializeField] CharacterCustomSelectable startSelected;
    [SerializeField] Characters characters;
    CharacterCustomSelectable activeSelectable;
    string inputDevice;
    bool activeController;

    bool active;

    bool blockedInput;
    float lastInput;

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


    }

    void Update() 
    {
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

            if (lastInput<-0.5f)
            {
                CharacterCustomSelectable nextSelectable = activeSelectable.FindSelectableOnDown() as CharacterCustomSelectable;
                if (nextSelectable!=null)
                {    
                    activeSelectable.DeselectCustomSelectable();
                    activeSelectable = nextSelectable;
                    activeSelectable.SelectCustomSelectable();
                    StartCoroutine (BlockInput());
                }    
            }

            else if (lastInput>0.5f)
            {    
                CharacterCustomSelectable nextSelectable = activeSelectable.FindSelectableOnUp() as CharacterCustomSelectable;

                if (nextSelectable!=null)
                {
                    activeSelectable.DeselectCustomSelectable();
                    activeSelectable = nextSelectable;
                    activeSelectable.SelectCustomSelectable();
                    StartCoroutine (BlockInput());
                }
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

        //Find out input device
        string key = "";
        if (characters == Characters.Child)
            key = "Child";
        else
            key = "Parent";

        
        inputDevice = GameData.GetData<PlayerData>(key).tempInputDevice;
        activeController = CheckForController();

        Debug.Log ("Selectables Length"+ selectables.Length);

        for (int i = 0; i<selectables.Length; i++)
        {
            //Set Data to selectables
            selectables[i].SetData(inputDevice, activeController, characters);
        }
    }

}
