using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScreenData
{
    public string key;
    public Characters character;
    public List<string> setInputDevices;
    public CharacterSelect characterSelect;
    public GameObject inputSelectToHide;

    public SelectScreenData(Characters character, List<string> setInputDevices, CharacterSelect characterSelect,GameObject inputSelectToHide)
    {
        this.character = character;
        this.setInputDevices = setInputDevices;
        this.characterSelect = characterSelect;
        this.inputSelectToHide = inputSelectToHide;

        if (character == Characters.Child)
            key = "Child";
        else
            key = "Parent";
    }
}

public class SelectScreenManager : MonoBehaviour
{
    
    [SerializeField] CharacterSelect characterSelectChild, characterSelectParent;
    [SerializeField] GameObject spawnPositionChild, spawnPositionParent;
    [SerializeField] Transform cameraPosition;
    [SerializeField] BackButton backButton;
    [SerializeField] GameObject inputSelectToHideChild, inputSelectToHideParent;

    List<string> setInputDevices = new List<string>();
    MenuState child,parent;
    SelectScreenData parent_selectScreenData, child_SelectScreenData;
    void Awake()
    {   

        //Spawn Characters and ajdust Camera
        CharacterInstantiator.InstantiateCharacter(Characters.Child, out GameObject characterChild, spawnPositionChild.transform, true);
        CharacterInstantiator.InstantiateCharacter(Characters.Parent, out GameObject characterParent, spawnPositionParent.transform,true);
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex",0);
        Camera.main.GetComponent<CameraManager>().SetCameraAsMain();
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);

        //Initialise StateMachine
        parent_selectScreenData = new SelectScreenData(Characters.Parent,setInputDevices,characterSelectParent,inputSelectToHideParent);
        child_SelectScreenData = new SelectScreenData(Characters.Child,setInputDevices,characterSelectChild,inputSelectToHideChild);
        child = new WaitForKeyInput(child_SelectScreenData);
        parent = new WaitForKeyInput(parent_selectScreenData);

        //Deactivate characterSelect overlay
        characterSelectChild.gameObject.SetActive(false);
        characterSelectParent.gameObject.SetActive(false);

        //Add Listeners to buttons
        parent_selectScreenData.characterSelect.readyButtonHalf.onClick.AddListener(()=>SetReady(Characters.Parent));
        child_SelectScreenData.characterSelect.readyButtonHalf.onClick.AddListener(()=>SetReady(Characters.Child));
    }

   void Update()
   {    
        child = child.UpdateMenu();
        parent = parent.UpdateMenu();

        if (child is Ready && parent is Ready)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }
 
   }

   void SetReady (Characters characters)
   {
        if (characters == Characters.Child)
            child = new Ready(child_SelectScreenData);
        else
            parent = new Ready(parent_selectScreenData);
   }
    
}

public abstract class MenuState
{   
    protected SelectScreenData dataPack;
    public abstract MenuState UpdateMenu();

    public MenuState(SelectScreenData dataPack)
    {
        this.dataPack = dataPack;
    }
}

public class WaitForKeyInput: MenuState
{   
    public WaitForKeyInput(SelectScreenData data) : base(data) {}
        public override MenuState UpdateMenu()
        {

            string inputDevice = "";
            if (dataPack.character == Characters.Child)
                inputDevice = GetInputDevice("X");
            else
                inputDevice = GetInputDevice("Y");
            
            if (inputDevice!="")
            {
                    Debug.Log ("Set Input device to "+inputDevice);
                    dataPack.setInputDevices.Add (inputDevice);
                    GameData.GetData<PlayerData>(dataPack.key).tempInputDevice = inputDevice;

                    dataPack.inputSelectToHide.SetActive(false);
                    dataPack.characterSelect.gameObject.SetActive(true);

                    dataPack.characterSelect.SetData(inputDevice,dataPack.character);

                    return new CustomiseCharacter(dataPack);
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
                else if (Input.GetAxis("K1 Horizontal")!=0 && dataPack.character == Characters.Child||
                        Input.GetAxis("K1 Vertical")!=0 && dataPack.character == Characters.Child) // First Keyboard Layout
                    inputDevice=  "K1";
                else if (Input.GetAxis("K2 Horizontal")!=0 && dataPack.character == Characters.Parent ||
                        Input.GetAxis("K2 Vertical")!=0 && dataPack.character == Characters.Parent) // Second Keyboard Layout
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
    public CustomiseCharacter(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {
        if (Input.GetButtonDown(dataPack.characterSelect.GetInputDevice()+"A"))
        {
            return new Ready(dataPack);
        }
              
        return this;
    }
}

public class Ready: MenuState
{
    public Ready(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {   
        dataPack.characterSelect.SetReadyButtonHalf(true);
        CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().SetInteger("ChildIndex",1);
        return this;
    }
}


