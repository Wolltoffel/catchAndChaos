using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSelector : MonoBehaviour
{   

    [SerializeField]ControllerUI controllerUI;
    List<string> setInputDevices = new List<string>();

    IEnumerator Start()
    {
        yield return WaitForKeyInput(1);
        yield return WaitForKeyInput (2);

        yield return null;
    }

    IEnumerator WaitForKeyInput(int playerIndex)
    {
        while (true)
        {   
            yield return null;
            string inputDevice = GetInputDevice();
            if (inputDevice!="")
            {   
                setInputDevices.Add (inputDevice);
                GameData.GetData<PlayerAgent>($"Player {playerIndex}").inputDevice = inputDevice; //Set Input Device for Player
                controllerUI.controllerSet(playerIndex,inputDevice);
                yield return null;
                break; 
            }
        }
    }

    string GetInputDevice()
    {
            string inputDevice = "";

            if (Input.GetButtonDown("J1A")) //Controller 1
               inputDevice =  "J1";
            else if (Input.GetButtonDown("J2A")) //Controller 2
                inputDevice=  "J2";
            else if (Input.GetButtonDown("J3A"))  //Controller 3
                inputDevice=  "J3";
           else if (Input.GetButtonDown("J4A")) //Controller 4
                inputDevice= "J4";
            else if (Input.GetButtonDown("K1A")) // First Keyboard Layout
                inputDevice=  "K1";
            else if (Input.GetButtonDown("K2A")) // Second Keyboard Layout
                inputDevice=  "K2";


            //Check if input device is already set
            for (int i = 0; i<setInputDevices.Count;i++)
            {
                    if (inputDevice==setInputDevices[i])
                    {
                        return "";
                    }
            }    
            
            return inputDevice;
    }
}
