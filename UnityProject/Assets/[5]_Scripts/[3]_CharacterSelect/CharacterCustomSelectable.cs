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

public class CharacterCustomSelectable: CustomSelectable
{
    [Header ("Buttons")]
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;

    string inputDevice;
    bool activeController;
    Characters characters;


    protected override void Start()
    {
        previousButton.onClick.AddListener (()=> Switch(Step.Prev));
        nextButton.onClick.AddListener (()=> Switch(Step.Next));
        interactable = false;
    }

    private void Update() 
    {
        WaitForControllerInputSwitch();
    }

    public void SetData(string inputDevice, bool activeController, Characters characters = Characters.Child)
    {
        this.inputDevice = inputDevice;
        this.activeController = activeController;
        this.characters = characters;
    }
    
    void Switch(Step step)
    {
        PropertyHandler.SwitchProperty(characters,step);
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
