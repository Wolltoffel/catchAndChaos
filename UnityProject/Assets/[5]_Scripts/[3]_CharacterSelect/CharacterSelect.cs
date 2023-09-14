using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]Characters characters;
    [SerializeField] Button prevController,nextController;
    [SerializeField] Button prevKeyboard,nextKeyboard;

    [Header ("ReadyButton")]
    [SerializeField] public GameObject activatedreadyButtonHalf;
    [SerializeField] public Button readyButtonHalf;

    string inputDevice;
    bool activeController;
    ButtonSwitcher buttonSwitcher;

    public void Awake()
    {   
        //Add Listener to all Button Variants        
        prevController.onClick.AddListener (()=> SwitchCharacter(Step.Prev));
        prevKeyboard.onClick.AddListener (()=> SwitchCharacter(Step.Prev));
        nextController.onClick.AddListener (()=> SwitchCharacter(Step.Next));
        nextKeyboard.onClick.AddListener (()=> SwitchCharacter(Step.Next));

        ButtonSwitchData prev = new ButtonSwitchData(prevController,prevKeyboard);
        ButtonSwitchData next = new ButtonSwitchData(nextController,nextKeyboard);
        buttonSwitcher = gameObject.AddComponent<ButtonSwitcher>();
        buttonSwitcher.buttons.Add(prev);
        buttonSwitcher.buttons.Add(next);

    }
    public void SetData(string inputDevice, Characters characters)
    {
        this.inputDevice = inputDevice;
        this.characters = characters;
        activeController = CheckForController();
    }

    public string GetInputDevice()
    {
        return inputDevice;
    }

    void  Update()
    {
        if (activeController)
            ProcessControllerInputsForSelection();

    }

    void ProcessControllerInputsForSelection()
    {
         if (Input.GetButtonDown(inputDevice+"BumperR"))
                SwitchCharacter (Step.Next);
                
        else if (Input.GetButtonDown(inputDevice+"BumperL"))
                SwitchCharacter (Step.Prev);
    }
    
    void SwitchCharacter(Step step)
    {
        if (characters == Characters.Child)
            GameData.GetData<PlayerData>("Child").characterAssets.UpdateCharacterPrefab(step);
        else if (characters == Characters.Parent)
            GameData.GetData<PlayerData>("Parent").characterAssets.UpdateCharacterPrefab(step);

        CharacterInstantiator.ReplaceCharacter(characters, out GameObject character, true);
    }

    bool CheckForController()
    {
        for (int i = 1; i<=4;i++)
        {
            if (inputDevice == "J"+i)
                return true;
        }
        return false;

    }

    public void SetReadyButtonHalf(bool active)
    {
        readyButtonHalf.gameObject.SetActive(!active);
        activatedreadyButtonHalf.SetActive(active);
    }
}
