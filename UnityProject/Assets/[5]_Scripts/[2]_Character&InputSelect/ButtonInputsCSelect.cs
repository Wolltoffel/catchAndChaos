using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInputsCSelect : MonoBehaviour
{
    [SerializeField] GameObject controller,keyboard;
    

    // Update is called once per frame
    void Update()
    {
        if (GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice == InputDevice.Keyboard)
        {
            controller.SetActive(false);
            keyboard.SetActive(true);
        }
            
        else
        {
            keyboard.SetActive(false);
            controller.SetActive(true);
        }
            
    }
}
