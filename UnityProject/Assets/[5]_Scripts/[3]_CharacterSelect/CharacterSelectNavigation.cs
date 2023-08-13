using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectNavigation : MonoBehaviour
{
    [SerializeField]CustomSelectable [] selectables;
    [SerializeField] CustomSelectable startSelected;
    [SerializeField] Characters characters;
    CustomSelectable activeSelectable;
    string inputDevice;
    bool activeController;

    public bool isConfirmed;

    void Start()
    {
        activeSelectable = startSelected;
        activeSelectable.SelectCustomSelectable();
        
        //Find out input device
        string key = "";
        if (characters == Characters.Child)
            key = "Child";
        else
            key = "Parent";

        inputDevice = GameData.GetData<PlayerData>(key).tempInputDevice;
        bool activeController = CheckForController();
        
        for (int i = 0; i<selectables.Length; i++)
        {
            //Set Data to selectables
            selectables[i].SetData(inputDevice, activeController, characters);
            //Deactivate selectables
            selectables[i].gameObject.SetActive (false);
        }

    }

    void Update() 
    {
        WaitForControllerInput();
    }

    void WaitForControllerInput()
    {
        if (activeController)
        {
            if (Input.GetAxis(inputDevice+" Vertical")>0)
            {
                activeSelectable.DeselectCustomSelectable();
                activeSelectable = activeSelectable.FindSelectableOnDown() as CustomSelectable;
                activeSelectable.Select();
            }
            else if (Input.GetAxis(inputDevice+" Vertical")<0)
            {
                activeSelectable.DeselectCustomSelectable();
                activeSelectable = activeSelectable.FindSelectableOnUp() as CustomSelectable;
                activeSelectable.Select();
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
        for (int i = 0; i<selectables.Length; i++)
        {
            selectables[i].gameObject.SetActive(true);
        }   
    }

}
