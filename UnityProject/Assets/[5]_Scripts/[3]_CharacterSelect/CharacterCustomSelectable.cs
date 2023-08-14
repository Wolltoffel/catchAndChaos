using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

enum Bumper 
{
    Left, Right
}

enum ControlMethod
{
    ScrollThrough, SetValues
}

public class CharacterCustomSelectable: CustomSelectable
{
    [SerializeField] ControlMethod controlMethod;

    [Header ("Buttons")]
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;


    [Header ("Set Values")]
    [SerializeField] int valueIntL;
    [SerializeField] int valueIntR;
    [SerializeField] Gender valueGenderL, valueGenderR;

    [Header("Other")]
    [SerializeField] ModelProperty modelProperty;

    string inputDevice;
    bool activeController;
    Characters characters;


    PropertyHandler propertyHandler = new PropertyHandler();
    protected override void Start()
    {
        previousButton.onClick.AddListener (()=> Switch(Step.Prev));
        nextButton.onClick.AddListener (()=> Switch(Step.Next));
        interactable = false;
    }

    private void Update() 
    {
        if (controlMethod == ControlMethod.ScrollThrough)
            WaitForControllerInputSwitch();
        else if (controlMethod == ControlMethod.SetValues)
            WaitForControllerInputSet();
    }

    public void SetData(string inputDevice, bool activeController, Characters characters = Characters.Child)
    {
        this.inputDevice = inputDevice;
        this.activeController = activeController;
        this.characters = characters;
    }
    
    void Switch(Step step)
    {
        propertyHandler.SwitchProperty(characters, modelProperty, step);
        CharacterSpawner.UpdateCharacter(characters);
    }

    void Set (Bumper bumper)
    {
        if (modelProperty == ModelProperty.Gender){
            if (bumper == Bumper.Left)
            {
                propertyHandler.SetProperty(characters,modelProperty,valueGender:valueGenderL);
            }
            else if (bumper == Bumper.Right)
            {
                propertyHandler.SetProperty(characters,modelProperty,valueGender:valueGenderR);
            }
        }
        else
        {   
            if (bumper == Bumper.Left)
            {
                propertyHandler.SetProperty(characters,modelProperty,valueIntL);
            }
            else if (bumper == Bumper.Right)
            {
                propertyHandler.SetProperty(characters,modelProperty,valueIntR);
            }
        }

       CharacterSpawner.UpdateCharacter(characters);
    }

    void WaitForControllerInputSwitch()
    {
        if (activeController && active)
        {          
            if (Input.GetButtonDown(inputDevice+"BumperR"))
            {
                Switch (Step.Next);
            }
                
            else if (Input.GetButtonDown(inputDevice+"BumperL"))
            {
                Switch (Step.Prev);
            }
                
        }

    }

    void WaitForControllerInputSet()
    {
        if (activeController && active)
        {          
            if (Input.GetButtonDown(inputDevice+"BumperR"))
            {
                Set (Bumper.Right);
            }
                
            else if (Input.GetButtonDown(inputDevice+"BumperL"))
            {
                Set (Bumper.Left);
            }
                
        }

    }
    public void SelectCustomSelectable()
    {
        //DoStateTransition (SelectionState.Pressed,true);
        //OnPointerDown(new (EventSystem.current));
        GetComponent<Image>().color = colors.highlightedColor;
        active = true;
    }

    public void DeselectCustomSelectable()
    {
        //DoStateTransition (SelectionState.Normal,true);
        GetComponent<Image>().color = colors.normalColor;
        active = false;
    }


}
