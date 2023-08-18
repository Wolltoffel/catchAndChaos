using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum InputSelectPrompt
{
    K1, K2
}

public class InputSelectUIManager : MonoBehaviour
{
    [SerializeField]GameObject childInputJ, childInputK1, childInputK2, childKeyBoardHeading;
    [SerializeField]GameObject parentInputJ,parentInputK1, parentInputK2,parentKeyBoardHeading;

    void Update()
    {
        UpdatePrompts();
    }

    public void HideAll(Characters characters)
    {
        if (characters == Characters.Child)
        {
            childInputJ.SetActive(false);
            childInputK1.SetActive(false);
            childInputK2.SetActive(false);
            parentKeyBoardHeading.SetActive (false);
        }
        else
        {
            parentInputJ.SetActive(false);
            parentInputK1.SetActive(false);
            parentInputK2.SetActive(false);
            childKeyBoardHeading.SetActive (false);
        }
    }

    public void Hide (InputSelectPrompt inputSelectPrompt, Characters characters)
    {
        if (characters == Characters.Child)
        {
            if (inputSelectPrompt == InputSelectPrompt.K1)
            {
                childInputK1.SetActive(false);
            }
            else if (inputSelectPrompt == InputSelectPrompt.K2)
            {
                childInputK2.SetActive(false);
            }
        }
        else if (characters == Characters.Parent)
        {
            if (inputSelectPrompt == InputSelectPrompt.K1)
            {
                parentInputK1.SetActive(false);
            }
            else if (inputSelectPrompt == InputSelectPrompt.K2)
            {
                parentInputK2.SetActive(false);
            }
        }
    }

    void SetKeyBoardPrompts(bool active)
    {
        parentInputK1.SetActive (active);
        parentInputK2.SetActive (active);
        childInputK1.SetActive (active);
        childInputK2.SetActive (active);
        childKeyBoardHeading.SetActive (active);
        parentKeyBoardHeading.SetActive (active);
    }

    void SetControllerPrompts(bool active)
    {
        parentInputJ.SetActive (active);
        childInputJ.SetActive (active);
    }



    public void UpdatePrompts()
    {
        InputDevice inputDevice = GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice;
        if (inputDevice == InputDevice.Controller)
        {
            SetKeyBoardPrompts(false);
            SetControllerPrompts (true);
        }
        else if (inputDevice == InputDevice.Keyboard)
        {
            SetKeyBoardPrompts(true);
            SetControllerPrompts (false);
        }
    }
}
