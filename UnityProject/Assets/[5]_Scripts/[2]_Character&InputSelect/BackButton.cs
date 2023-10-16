using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField]ScreenType screenToJumpTo;
    Button button;
    Image image;
    public List<string> ignoredInputs = new List<string>();

    void  Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>GoBack());
        image = GetComponent<Image>();
    }

    void Update()
    {   
        WaitForControllerInput();

        if (GameData.GetData<LastInputDeviceData>("LastInputDeviceData").inputDevice==InputDevice.Controller)
        {
            button.enabled = false;
            image.enabled = false;
            GetComponent<ButtonAddition>().enabled = false;
        }
            
        else
        {
            button.enabled = true;
            image.enabled = true;
            GetComponent<ButtonAddition>().enabled = true;
        }
            
    }

    public void AddIgnoreController (string ignoredController)
    {
        for (int i = 0; i < ignoredInputs.Count; i++)
        {
            if (ignoredInputs[i].Equals(ignoredController))
            {
                return;
            }
                
        }
        
        ignoredInputs.Add(ignoredController);
    }

    void GoBack()
    {
        ScreenSwitcher.SwitchScreen(screenToJumpTo, ScreenSwitcher.LoadingScreenType.Off);
    }

    public void SetScreenScreenToJumpTo(ScreenType screenToJumpTo)
    {
        this.screenToJumpTo = screenToJumpTo;
    }

    void WaitForControllerInput()
    {
        for (int i = 0; i < ignoredInputs.Count; i++)
        {   

            string buttonName = ignoredInputs[i]+"B";
            if (Input.GetButtonDown(buttonName))
            {
                ignoredInputs.Remove(ignoredInputs[i]);
                return;
            }
        }

        if (Input.GetButtonDown("AB"))
        {
            GoBack();
        }     
    }
                
        
}
