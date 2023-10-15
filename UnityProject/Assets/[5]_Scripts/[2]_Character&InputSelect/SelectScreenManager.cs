using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScreenData
{
    public string key;
    public Characters character;
    public List<string> setInputDevices;
    public CharacterSelect characterSelect;
    public GameObject inputSelectToHide;
    public BackButton backButton;
    public GameObject butttonInfo;
    public string selectedInputDevice;


    public SelectScreenData(Characters character, List<string> setInputDevices, CharacterSelect characterSelect,GameObject inputSelectToHide,BackButton backButton, GameObject buttonInfo)
    {
        this.character = character;
        this.setInputDevices = setInputDevices;
        this.characterSelect = characterSelect;
        this.inputSelectToHide = inputSelectToHide;
        this.backButton = backButton;
        this.butttonInfo = buttonInfo;

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

    [SerializeField] GameObject buttonInfo;

    [Header ("Character Select")]   
    [SerializeField] GameObject inputSelectToHideChild, inputSelectToHideParent;
    //[SerializeField]ReadyButton readyButton;

    List<string> setInputDevices = new List<string>();
    MenuState child,parent;
    SelectScreenData parent_selectScreenData, child_SelectScreenData;
    void Start()
    {   
        //Adjust Camera
        Camera.main.GetComponent<CameraManager>().SetCameraAsMain();
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);

        //Initialise StateMachine
        parent_selectScreenData = new SelectScreenData(Characters.Parent,setInputDevices,characterSelectParent,inputSelectToHideParent,backButton,buttonInfo);
        child_SelectScreenData = new SelectScreenData(Characters.Child,setInputDevices,characterSelectChild,inputSelectToHideChild,backButton,buttonInfo);
        child = new WaitForKeyInput(child_SelectScreenData);
        parent = new WaitForKeyInput(parent_selectScreenData);

        //Deactivate characterSelect overlay
        characterSelectChild.SetInteractable(false);
        characterSelectParent.SetInteractable(false);

        //Deactivate ButtonInfo
        buttonInfo.SetActive(false);
        
    }

   void Update()
   {    
        child = child.UpdateMenu();
        parent = parent.UpdateMenu();

        if (child is ReadyToSwitchScreen && parent is ReadyToSwitchScreen)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }

        if (child is CustomiseCharacterSingle | parent is CustomiseCharacterSingle)
            buttonInfo.SetActive(true);
        else
            buttonInfo.SetActive(false);

   }

   void SetReady (Characters characters)
   {
        if (characters == Characters.Child && !(child is Ready)&&!(child is ReadyToSwitchScreen))
        {
            child = new Ready(child_SelectScreenData);
        }
            
        else if (characters == Characters.Parent && !(parent is Ready)&&!(parent is ReadyToSwitchScreen))
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
                    dataPack.setInputDevices.Add (inputDevice);
                    dataPack.selectedInputDevice = inputDevice;
                    GameData.GetData<PlayerData>(dataPack.key).tempInputDevice = inputDevice;

                    dataPack.inputSelectToHide.SetActive(false);
                    dataPack.characterSelect.SetInteractable(true);

                    dataPack.characterSelect.SetData(inputDevice,dataPack.character);

                    return new CustomiseCharacterSingle(dataPack);
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

public class CustomiseCharacterSingle: MenuState
{
    public CustomiseCharacterSingle(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {
        dataPack.backButton.AddIgnoreController(dataPack.selectedInputDevice);

         if (Input.GetButtonDown(dataPack.characterSelect.GetInputDevice()+"A")|Input.GetKeyDown(KeyCode.Return))
        {
            dataPack.characterSelect.HideCharacterSelectElements();

            if(dataPack.character==Characters.Child)
                CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().SetInteger("ChildIndex",11);
            else
                CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().SetInteger("MomIndex",11);
            
            return new Ready(dataPack);
        }

        if (Input.GetButtonDown(dataPack.characterSelect.GetInputDevice()+"B"))
        {
            dataPack.setInputDevices.Remove (dataPack.selectedInputDevice);
            dataPack.inputSelectToHide.SetActive(true);
            dataPack.characterSelect.SetInteractable(false);;

            return new WaitForKeyInput(dataPack);
        }
                    
        return this;
    }

}

public class Ready: MenuState
{
    bool startedTimer = false;
    float timer;
    public Ready(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {   
        AnimationClip animationClip = CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip;


        if (animationClip.name.Equals("animReady") && startedTimer==false)
        {
            startedTimer = true;
            timer = animationClip.length;
        }

        if (startedTimer)
            timer -=Time.deltaTime;

        if (timer<=0.01f && startedTimer)
            return new ReadyToSwitchScreen(dataPack);

        return this;
    }

}

public class ReadyToSwitchScreen: MenuState
{
    public ReadyToSwitchScreen(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {   
        return this;
    }
}


