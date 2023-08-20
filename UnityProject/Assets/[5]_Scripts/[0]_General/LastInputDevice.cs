using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
         if (Input.anyKeyDown)
         {
            Debug.Log("anykey");
            // Get the last input device used
            string lastInputDevice = Input.inputString;
            
            LastInputDeviceData lastInputDeviceData =new LastInputDeviceData(InputDevice.Keyboard);

            if (lastInputDevice!= ""||Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1)||Input.GetMouseButtonDown(2)
                || Input.GetAxis ("K2 Horizontal")!=0|| Input.GetAxis("K2 Vertical") !=0)
            {
                lastInputDeviceData = new LastInputDeviceData(InputDevice.Keyboard);
            }
            else
            {
                lastInputDeviceData = new LastInputDeviceData(InputDevice.Controller);
            }


            GameData.SetData(lastInputDeviceData,"LastInputDeviceData");
        }
    }
    
    void CheckButtonBelow()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
