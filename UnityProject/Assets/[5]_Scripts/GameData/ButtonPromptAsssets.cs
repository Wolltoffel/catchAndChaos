using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDevice
{
    Keyboard, Controller
}

[System.Serializable]
public class ButtonPromptData
{
    public GameObject buttonAsset;
    public string buttonName;
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/ButtonPromptAssets")]
public class ButtonPromptAsssets : StaticData
{
   public ButtonPromptData[] controller;
   public ButtonPromptData[] keyboard;


    public GameObject GetButtonByName (string name, InputDevice inputDevice)
    {
        if (inputDevice == InputDevice.Controller)
            return SearchList(name, controller).buttonAsset;
        else if (inputDevice == InputDevice.Controller)
            return SearchList(name, keyboard).buttonAsset;
        else
            throw new System.Exception ("No input device selected");      
    }

    ButtonPromptData SearchList(string name, ButtonPromptData[] buttonPromptDatas)
    {
        for (int i= 0 ; i<buttonPromptDatas.Length; i++)
        {
            if (buttonPromptDatas[i].buttonName == name)
                return buttonPromptDatas[i];
        }
        return null;
    }
}

