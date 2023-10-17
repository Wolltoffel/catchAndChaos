using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInputsCSelect : MonoBehaviour
{
    [SerializeField] GameObject controllerScheme,keyboardScheme;
    

    void Awake()
    {
        SwitchLayouts();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchLayouts();
    }

    void SwitchLayouts()
    {
        if (GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice == InputDevice.Keyboard)
        {
            controllerScheme.SetActive(false);
            keyboardScheme.SetActive(true);
        }
            
        else
        {
            keyboardScheme.SetActive(false);
            controllerScheme.SetActive(true);
        } 
    }
}
