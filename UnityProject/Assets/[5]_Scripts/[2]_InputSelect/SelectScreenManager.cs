using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomisationData
{
    public InputSelectUIManager inputSelectUI;
    public CharacterSelectNavigation characterSelectNavigation;
    public string key;
    public Characters character;
    public List<string> setInputDevices;

    public BackButton backButton;


    public CustomisationData(Characters character,InputSelectUIManager inputSelectUI,
    CharacterSelectNavigation characterSelectNavigation,List<string> setInputDevices,
    BackButton backButton)
    {
        this.character = character;
        if (character == Characters.Child)
            key = "Child";
        else
            key = "Parent";
        this.inputSelectUI = inputSelectUI;
        this.setInputDevices = setInputDevices;

        this.characterSelectNavigation = characterSelectNavigation;
        this.backButton = backButton;
    }
}

public class SelectScreenManager : MonoBehaviour
{
    
    [SerializeField]InputSelectUIManager inputSelectUI;
    [SerializeField] CharacterSelectNavigation childUI, parentUI;
    [SerializeField] GameObject spawnPositionChild, spawnPositionParent;
    [SerializeField] Transform cameraPosition;
    [SerializeField] BackButton backButton;

    List<string> setInputDevices = new List<string>();
    MenuState child,parent;
    CustomisationData parent_CustomisationData, child_CustomisationData;

    void Awake()
    {   
        //Spawn Characters and ajdust Camera
        CharacterInstantiator.InstantiateCharacter(Characters.Child, out GameObject characterChild, spawnPositionChild.transform, true);
        CharacterInstantiator.InstantiateCharacter(Characters.Parent, out GameObject characterParent, spawnPositionParent.transform,true);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);

        //Initialise StateMachine
        parent_CustomisationData = new CustomisationData(Characters.Parent,inputSelectUI,parentUI,setInputDevices,backButton);
        child_CustomisationData = new CustomisationData(Characters.Child,inputSelectUI,childUI,setInputDevices,backButton);
        child = new WaitForKeyInput(child_CustomisationData);
        parent = new WaitForKeyInput(parent_CustomisationData);
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

            string inputDevice = "";
            if (dataPack.character == Characters.Child)
                inputDevice = GetInputDevice("X");
            else
                inputDevice = GetInputDevice("Y");
            
                if (inputDevice!="")
                {   
                    dataPack.setInputDevices.Add (inputDevice);
                    //Set Input Device for Player
                    GameData.GetData<PlayerData>(dataPack.key).tempInputDevice = inputDevice; 
                    dataPack.inputSelectUI.HideAll(dataPack.character);
                    dataPack.characterSelectNavigation.ActivateCharacterSelection();
                    dataPack.backButton.SetScreenScreenToJumpTo(ScreenType.CharacterInputSelect);
                    return new CustomiseCharacter(dataPack);
                }

                for (int i = 0; i<dataPack.setInputDevices.Count;i++)
                {
                    if (dataPack.setInputDevices[i] == "K1")
                        dataPack.inputSelectUI.Hide(InputSelectPrompt.K1,dataPack.character);
                    else if (dataPack.setInputDevices[i] == "K2")
                        dataPack.inputSelectUI.Hide(InputSelectPrompt.K2,dataPack.character);
                }

            return this;
        }

        string GetInputDevice(string buttton)
        {
                string inputDevice = "";
                if (Input.GetButtonDown("J1"+buttton)) //Controller 1
                    inputDevice =  "J1";
                else if (Input.GetButtonDown("J2"+buttton)) //Controller 2
                    inputDevice=  "J2";
                else if (Input.GetButtonDown("J3"+buttton))  //Controller 3
                    inputDevice=  "J3";
                else if (Input.GetButtonDown("J4"+buttton)) //Controller 4
                    inputDevice= "J4";
                else if (Input.GetButtonDown("K1"+buttton)) // First Keyboard Layout
                    inputDevice=  "K1";
                else if (Input.GetButtonDown("K2"+buttton)) // Second Keyboard Layout
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
          CharacterSelectNavigation characterSelectNavigation =  dataPack.characterSelectNavigation;
        if (characterSelectNavigation.isConfirmed)
            return new Ready(dataPack);
            
            return this;
    }
}

public class Ready: MenuState
{
    public Ready(CustomisationData data) : base(data) {}
    public override MenuState UpdateMenu()
    {   
        dataPack.characterSelectNavigation.gameObject.SetActive (false);
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex",0);
        return this;
    }
}


