using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]Characters characters;
    [SerializeField] Button prevController,nextController;
    [SerializeField] Button prevKeyboard,nextKeyboard;
    [SerializeField] ScrollSelection scrollSelection;

    bool interactable;

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

        scrollSelection.SetCharacter(characters);

    }

    public void SetInteractable(bool interactableInput)
    {
        interactable = interactableInput;

        prevController.gameObject.SetActive(interactableInput);
        prevKeyboard.gameObject.SetActive(interactableInput);
        nextController.gameObject.SetActive(interactableInput);
        nextKeyboard.gameObject.SetActive(interactableInput);
        
        if (buttonSwitcher==null)
            buttonSwitcher = gameObject.AddComponent<ButtonSwitcher>();
        
        buttonSwitcher.enabled = interactableInput;
    
            

        scrollSelection.gameObject.SetActive(interactableInput);
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
         if (Input.GetButtonDown(inputDevice+"BumperR") && interactable)
         {
            nextController.onClick.Invoke();
            
            StartCoroutine(ButtonBlink(nextController,new Vector2(1.15f,1.15f)));
         }            
        else if (Input.GetButtonDown(inputDevice+"BumperL") && interactable)
        {
            prevController.onClick.Invoke();
            StartCoroutine(ButtonBlink(prevController,new Vector2(1.15f,1.15f)));
        }

    }
    
    void SwitchCharacter(Step step)
    {
        if (characters == Characters.Child)
            GameData.GetData<PlayerData>("Child").characterAssets.UpdateCharacterPrefab(step);
        else if (characters == Characters.Parent)
            GameData.GetData<PlayerData>("Parent").characterAssets.UpdateCharacterPrefab(step);

        scrollSelection.Slide(step);

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
    IEnumerator ButtonBlink(Button button,Vector2 targetScale)
    {
        button.transform.localScale = new Vector2(1,1);
        Vector2 currentScale = button.transform.localScale;
        Vector2 startScale = currentScale;

        //Scale up
        float startTime = Time.time;
        while (Vector2.Distance(currentScale, targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.2f;
            currentScale = Vector2.Lerp(currentScale, targetScale, t);
            button.transform.localScale = currentScale;
            yield return null;
        }

        //Scale down
        startTime = Time.time;
        while (Vector2.Distance(currentScale, startScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.2f;
            currentScale = Vector2.Lerp(currentScale, startScale, t);
            button.transform.localScale = currentScale;
            yield return null;
        }
    }

    public void HideCharacterSelectElements()
    {
        prevController.gameObject.SetActive(false);
        prevKeyboard.gameObject.SetActive(false);
        nextController.gameObject.SetActive(false);
        nextKeyboard.gameObject.SetActive(false);
        scrollSelection.HighlightSelected();
        Destroy(this);
        Destroy(buttonSwitcher);
        buttonSwitcher = null;
    }

    public void HideCharacterModels()
    {
        scrollSelection.HideCharacters();
    }
}
