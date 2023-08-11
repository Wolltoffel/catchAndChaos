using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomisationData
{
    public CharacterSelectManager materialSelectorUI;
    public string key;
    public List<string> setInputDevices;

    public CustomisationData(string key,CharacterSelectManager materialSelectorUI,List<string> setInputDevices)
    {
        this.key = key;
        this.materialSelectorUI = materialSelectorUI;
        this.setInputDevices = setInputDevices;
    }
}

public class SelectScreenManager : MonoBehaviour
{
    List<string> setInputDevices = new List<string>();
    CustomisationData parent_characterCustomisationDataPack, child_CustomisationData;

    MenuState child,parent;

    [SerializeField]CharacterSelectManager materialSelectorUI;
    [SerializeField] Transform cameraPosition;

    void Start()
    {   
        //Spawn Characters and ajdust Camera

        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);

        parent_characterCustomisationDataPack = new CustomisationData("Parent",materialSelectorUI,setInputDevices);
        child_CustomisationData = new CustomisationData("Child",materialSelectorUI,setInputDevices);

        child = new WaitForKeyInput(child_CustomisationData);
        parent = new WaitForKeyInput(parent_characterCustomisationDataPack);
    }

   void Update()
   {
        child = child.UpdateMenu();
        parent = parent.UpdateMenu();

        if (child is Ready && parent is Ready)
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
   }

    
}

public abstract class MenuState
{   
    protected CustomisationData dataPack;
    public abstract MenuState UpdateMenu();

    public MenuState(CustomisationData dataPack)
    {
        this.dataPack = dataPack;
    }
}

public class WaitForKeyInput: MenuState
{   
    public WaitForKeyInput(CustomisationData data) : base(data) {}
        public override MenuState UpdateMenu()
        {
            string inputDevice = GetInputDevice();
                if (inputDevice!="")
                {   
                    dataPack.setInputDevices.Add (inputDevice);
                    //Set Input Device for Player
                    GameData.GetData<PlayerData>(dataPack.key).tempInputDevice = inputDevice; 
                    //UIHandler.UpdateUIToChaacterSelect;
                    return new CustomiseCharacter(dataPack);
                }

            return this;
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
                for (int i = 0; i<dataPack.setInputDevices.Count;i++)
                {
                        if (inputDevice==dataPack.setInputDevices[i])
                        {
                            return "";
                        }
                }     
                return inputDevice;
        }  
}

public class CustomiseCharacter: MenuState
{
    public CustomiseCharacter(CustomisationData data) : base(data) {}
    public override MenuState UpdateMenu()
    {
          CharacterSelectManager materialSelectorUI =   dataPack.materialSelectorUI;
        if (materialSelectorUI.child.isConfirmed && materialSelectorUI.parent.isConfirmed)
            return new Ready(dataPack);
            
            return this;
    }
}

public class Ready: MenuState
{
    public Ready(CustomisationData data) : base(data) {}
    public override MenuState UpdateMenu()
    {   
        return this;
    }
}


