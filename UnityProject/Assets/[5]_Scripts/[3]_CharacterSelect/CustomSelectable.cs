using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSelectable: Selectable
{
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;
    [SerializeField] ModelProperty modelProperty;

    string inputDevice;
    bool activeController;
    Characters characters;
    bool active;

    PropertyHandler propertyHandler = new PropertyHandler();

    private void Start()
    {
        previousButton.onClick.AddListener (()=> Switch(Step.Prev));
        nextButton.onClick.AddListener (()=> Switch(Step.Next));
    }

    private void Update() 
    {
        WaitForSwitch();
    }

    public void SetData(string inputDevice, bool activeController, Characters characters)
    {
        this.inputDevice = inputDevice;
        this.activeController = activeController;
        this.characters = characters;
    }
    
    void Switch(Step step)
    {
        propertyHandler.SetProperty(characters, modelProperty, step);
        CharacterSpawner.UpdateCharacter(characters);
    }

    void WaitForSwitch()
    {
        if (activeController && active)
        {

            if (Input.GetButtonDown(inputDevice+"BumperR"))
            {
                Debug.Log ("Next");
                Switch (Step.Next);
            }
                
            else if (Input.GetButtonDown(inputDevice+"BumperL"))
            {
                Debug.Log ("Prev");
                Switch (Step.Prev);
            }
                
        }

    }

    public void SelectCustomSelectable()
    {
        DoStateTransition (SelectionState.Pressed,true);
        active = true;
    }

    public void DeselectCustomSelectable()
    {
        DoStateTransition (SelectionState.Normal,true);
        active = false;
    }


}
