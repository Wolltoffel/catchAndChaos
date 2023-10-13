using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class LastInputDeviceData: Object
{
    public readonly InputDevice inputDevice;

    public LastInputDeviceData(InputDevice inputDevice)
    {
        this.inputDevice = inputDevice;
    }
} 


public class LastInputDevice : MonoBehaviour
{
    Vector3 lastMousePosition;

    private void  Awake()
    {
        GameData.SetData(new LastInputDeviceData(InputDevice.Keyboard),"LastInputDeviceData");
    }

    private void Update() 
    {
        RegisterLastDevice();
    }

    public void RegisterLastDevice()
    {   

        //Check last Inputs
         if (Input.anyKeyDown ||CheckControllerAxis())
         {

            EventSystem eventSystem = EventSystem.current;

            // Get the last input device used
            string lastInputDevice = Input.inputString;
            
            LastInputDeviceData lastInputDeviceData =new LastInputDeviceData(InputDevice.Keyboard);

            if (lastInputDevice!= ""|DetecMouseButtonPress()|CheckKeyboardKeys()| DetectMouseMovement())
            {
                    Cursor.visible = true;
                    lastInputDeviceData = new LastInputDeviceData(InputDevice.Keyboard);

                    if (eventSystem!=null
                        && GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice==InputDevice.Controller)
                            eventSystem.SetSelectedGameObject(null);

            }
            else
            {
                                    
                if (eventSystem!=null
                &&eventSystem.currentSelectedGameObject==null 
                && GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice==InputDevice.Keyboard)
                    eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);

                Cursor.visible = false;
                lastInputDeviceData = new LastInputDeviceData(InputDevice.Controller);
            }

            GameData.SetData(lastInputDeviceData,"LastInputDeviceData");
        }

    }


    bool CheckKeyboardKeys()
    {
            List<KeyCode> allKeyCodes = System.Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
            
            for (int i = 0; i<allKeyCodes.Count<KeyCode>();i++)
            {
                if (Input.GetKeyDown(allKeyCodes[i]) && !allKeyCodes[i].ToString().Contains("Joystick"))
                    return true;
            }
            return false;
    }

    bool DetecMouseButtonPress()
    {
        if (Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1)||Input.GetMouseButtonDown(2))
            return true;
        else
            return false;
    }

    bool DetectMouseMovement()
    {   
        bool mouseMovement;
        Vector3 newMousePosition = Input.mousePosition;
        
        if (Vector3.Distance(lastMousePosition,newMousePosition)<0.001f)
            mouseMovement = false;
        else
            mouseMovement = true;

        lastMousePosition = newMousePosition;
        return mouseMovement;   
    }

    bool CheckControllerAxis()
    {
        string[] controllerNumbers = new string[]{"J1","J2","J3","J4"};
        string[] axisNames = new string[]{"Horizontal","Vertical"};
        
        for (int i = 0; i < controllerNumbers.Length; i++)
        {
            for (int j = 0; j < axisNames.Length; j++)
            {
                if (Input.GetAxis($"{controllerNumbers[i]} {axisNames[j]}")>0.0001f)
                    return true;
            }
        }

        Debug.Log ("No Axis Movement detectd");
        return false;
    }


}
