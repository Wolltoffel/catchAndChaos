using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public enum InputDevice
{
    Keyboard, Controller
}

[System.Serializable]
public class ButtonPromptData
{
    public Sprite buttonAsset;
    public string buttonName;
}

[System.Serializable]
public class HintData
{
    public Sprite hintSprite;
    public string hintName;
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/ButtonPromptAssets")]
public class ButtonPromptAsssets : StaticData
{
   public ButtonPromptData[] controller;
   public ButtonPromptData[] keyboard;

   public HintData [] hints;


    public Sprite GetButtonSpriteByName (string name, InputDevice inputDevice)
    {
        if (inputDevice == InputDevice.Controller)
            return SearchButtonList(name, controller).buttonAsset;
        else if (inputDevice == InputDevice.Keyboard)
        {   
            return SearchButtonList(name, keyboard).buttonAsset;
        }
            
        else
            throw new System.Exception ("No input device selected");      
    }

    public Sprite GetButtonSpriteByName (string inputDeviceAndButton)
    {
        char firstLetter = inputDeviceAndButton[0];
        string searchkey  = "";
        InputDevice inputDevice = InputDevice.Keyboard;

        if (firstLetter == 'J')
        {
             inputDevice = InputDevice.Controller;
             searchkey = RemoveNumbersFromString(inputDeviceAndButton);
        }
           
        else if (firstLetter == 'K')
        {
            inputDevice = InputDevice.Keyboard;
            searchkey = inputDeviceAndButton;
        }
            
        if (inputDevice == InputDevice.Controller)
            return SearchButtonList(searchkey, controller).buttonAsset;
        else if (inputDevice == InputDevice.Keyboard)
            return SearchButtonList(searchkey, keyboard).buttonAsset;
        else
            throw new System.Exception ("No input device selected");      
    }

     public Sprite GetHintSpriteByName (string name)
    {
        return SearchHintList(name).hintSprite;
    }

    ButtonPromptData SearchButtonList(string name, ButtonPromptData[] buttonPromptDatas)
    {
        for (int i= 0 ; i<buttonPromptDatas.Length; i++)
        {
            if (buttonPromptDatas[i].buttonName == name)
                return buttonPromptDatas[i];
        }
        return null;
    }

    HintData SearchHintList(string name)
    {
        for (int i= 0 ; i<hints.Length; i++)
        {
            if (hints[i].hintName == name)
                return hints[i];
        }
        return null;
    }


    public string RemoveNumbersFromString(string input)
    {
        string result = Regex.Replace(input, @"\d", "");
        return result;
    }
    
}

