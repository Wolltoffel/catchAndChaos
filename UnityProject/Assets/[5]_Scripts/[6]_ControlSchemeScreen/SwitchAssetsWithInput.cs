using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAssetsWithInput : MonoBehaviour
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
            //controllerScheme.SetActive(false);
            //keyboardScheme.SetActive(true);
            controllerScheme.transform.localScale = Vector3.zero;
            keyboardScheme.transform.localScale = Vector3.one;
        }
            
        else
        {
            controllerScheme.transform.localScale = Vector3.one;
            keyboardScheme.transform.localScale = Vector3.zero;
        } 
    }
}
