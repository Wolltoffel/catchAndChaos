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
         {
            nextController.onClick.Invoke();
            
            StartCoroutine(ButtonBlink(nextController,new Vector2(1.15f,1.15f)));
         }
                
                
        else if (Input.GetButtonDown(inputDevice+"BumperL"))
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

        CharacterInstantiator.ReplaceCharacter(characters, out GameObject character, true);

        if (scrollSelection!=null)
            if (step == Step.Prev)
            scrollSelection.SlideLeft();
            else
                scrollSelection.SlideRight();
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
}
