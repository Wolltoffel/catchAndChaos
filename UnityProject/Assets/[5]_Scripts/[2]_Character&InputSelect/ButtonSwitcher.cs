using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[System.Serializable]
public class ButtonSwitchData
{
    public Button controller, keyboard;
    Button activeButton;

    public ButtonSwitchData(Button controller, Button keyboard)
    {
        this.controller = controller;
        this.keyboard = keyboard;
    }

    public void Switch (InputDevice inputDevice)
    {
        if (inputDevice == InputDevice.Controller)
        {
           controller.gameObject.SetActive(true);
           keyboard.gameObject.SetActive(false);
           activeButton = controller;

        }
        else if (inputDevice == InputDevice.Keyboard)
        {
            keyboard.gameObject.SetActive(true);
            controller.gameObject.SetActive(false);
            activeButton = keyboard;
        }
    }

    public void TurnOffAll()
    {
        keyboard.gameObject.SetActive(false);
        controller.gameObject.SetActive(false);
    }

    public Button GetActiveButton()
    {
        return activeButton;
    }

    public Button GetButton (InputDevice inputDevice)
    {
        if (inputDevice == InputDevice.Controller)
            return controller;
        else if (inputDevice == InputDevice.Keyboard)
            return keyboard;
        throw new System.Exception ("No such input device existent");
    }
}


public class ButtonSwitcher : MonoBehaviour
{
    [SerializeField] public List<ButtonSwitchData> buttons = new List<ButtonSwitchData>();
    private InputDevice recentInputDevice;

    void Start()
    {
        SwitchButtons();
    }

    void  Update()
    {
        InputDevice currentInputDevice = GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice;
        if (recentInputDevice !=currentInputDevice)
        {
            recentInputDevice = currentInputDevice;
            SwitchButtons();
        }
    }

    protected void SwitchButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Switch(recentInputDevice);
        }
    }

    List<Button> GetActiveButtons()
    {   
        List<Button> activeButtons = new List<Button>();

        for (int i = 0; i < buttons.Count; i++)
        {
           activeButtons.Add(buttons[i].GetActiveButton());
        }

        return activeButtons;
    }
    
}

