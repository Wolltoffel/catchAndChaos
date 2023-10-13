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

    public ReadyButton readyButton;

    public BackButton backButton;

    public SelectScreenData(Characters character, List<string> setInputDevices, CharacterSelect characterSelect,GameObject inputSelectToHide,ReadyButton readyButton,BackButton backButton)
    {
        this.character = character;
        this.setInputDevices = setInputDevices;
        this.characterSelect = characterSelect;
        this.inputSelectToHide = inputSelectToHide;
        this.readyButton = readyButton;
        this.backButton = backButton;

        if (character == Characters.Child)
            key = "Child";
        else
            key = "Parent";
    }
}

[System.Serializable]
public class ReadyButton
{
    public GameObject readdyButtonContainer;
    public Button controllerButton, keyboardButton;
    public GameObject highlightChild,highlightParent;

    public void Activate()
    {
        readdyButtonContainer.SetActive(true);
    }
    public void SetReady(Characters character)
    {
        if (character== Characters.Child)
            highlightChild.SetActive(true);
        else
            highlightParent.SetActive(true);
    }

    public bool GetReady(Characters characters)
    {
        if (characters== Characters.Child)
            return highlightChild.activeInHierarchy;
        else
            return highlightParent.activeInHierarchy;
    }

    public void Initialise()
    {
        controllerButton.onClick.AddListener(()=>{
            SetReady(Characters.Child);
            SetReady(Characters.Parent);
        });

        keyboardButton.onClick.AddListener(()=>{
            SetReady(Characters.Child);
            SetReady(Characters.Parent);
        });

        ButtonSwitchData buttonSwitchData = new ButtonSwitchData(controllerButton,keyboardButton);
        readdyButtonContainer.AddComponent<ButtonSwitcher>().buttons.Add(buttonSwitchData);
        
        readdyButtonContainer.SetActive(false);
        highlightChild.SetActive(false);
        highlightParent.SetActive(false);
        controllerButton.gameObject.SetActive(false);
        keyboardButton.gameObject.SetActive(false);
    }
}

public class SelectScreenManager : MonoBehaviour
{
    [SerializeField] CharacterSelect characterSelectChild, characterSelectParent;
    [SerializeField] GameObject spawnPositionChild, spawnPositionParent;
    [SerializeField] Transform cameraPosition;
    [SerializeField] BackButton backButton;

    [Header ("Character Select")]   
    [SerializeField] GameObject inputSelectToHideChild, inputSelectToHideParent;
    [SerializeField]ReadyButton readyButton;

    List<string> setInputDevices = new List<string>();
    MenuState child,parent;
    SelectScreenData parent_selectScreenData, child_SelectScreenData;
    void Awake()
    {   
        //Adjust Camera
        Camera.main.GetComponent<CameraManager>().SetCameraAsMain();
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);

        //Initialise StateMachine
        parent_selectScreenData = new SelectScreenData(Characters.Parent,setInputDevices,characterSelectParent,inputSelectToHideParent,readyButton,backButton);
        child_SelectScreenData = new SelectScreenData(Characters.Child,setInputDevices,characterSelectChild,inputSelectToHideChild,readyButton,backButton);
        child = new WaitForKeyInput(child_SelectScreenData);
        parent = new WaitForKeyInput(parent_selectScreenData);

        //Deactivate characterSelect overlay
        characterSelectChild.gameObject.SetActive(false);
        characterSelectParent.gameObject.SetActive(false);
        
        readyButton.Initialise();
    }

   void Update()
   {    
        child = child.UpdateMenu();
        parent = parent.UpdateMenu();

        if (child is ReadyToSwitchScreen && parent is ReadyToSwitchScreen)
        {
            ScreenSwitcher.SwitchScreen(ScreenType.GameScreen);
        }

        if (child is CustomiseCharacterSingle && parent is CustomiseCharacterSingle)
            readyButton.Activate();

        if (readyButton.GetReady(Characters.Child)&&readyButton.GetReady(Characters.Parent))
        {
            SetReady(Characters.Child);
            SetReady(Characters.Parent);
        }
 
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
                    GameData.GetData<PlayerData>(dataPack.key).tempInputDevice = inputDevice;

                    dataPack.inputSelectToHide.SetActive(false);
                    dataPack.characterSelect.gameObject.SetActive(true);

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

                Debug.Log (inputDevice);


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
        Characters otherCharacter;
        if (dataPack.character==Characters.Child)
            otherCharacter = Characters.Parent;
        else 
            otherCharacter = Characters.Child;
        
        if (dataPack.readyButton.readdyButtonContainer.activeInHierarchy)
        {
            dataPack.backButton.SetScreenScreenToJumpTo(ScreenType.CharacterInputSelect);
            return new CustomiseCharacterTogether(dataPack);
        }
                    
        return this;
    }

}

public class CustomiseCharacterTogether: MenuState
{
    public CustomiseCharacterTogether(SelectScreenData data) : base(data) {}
    public override MenuState UpdateMenu()
    {
        if (Input.GetButtonDown(dataPack.characterSelect.GetInputDevice()+"A")|dataPack.readyButton.GetReady(dataPack.character))
        {
            dataPack.readyButton.SetReady(dataPack.character);
            dataPack.characterSelect.HideCharacterSelect();

            if(dataPack.character==Characters.Child)
                CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().SetInteger("ChildIndex",11);
            else
                CharacterInstantiator.GetActiveCharacter(dataPack.character).GetComponent<Animator>().SetInteger("MomIndex",11);
            
            return new Ready(dataPack);
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


