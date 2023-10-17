using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class Pausescreen : MonoBehaviour
{
    [SerializeField] GameObject pauseScreenPrefab;
    [SerializeField] Transform parent;
    [SerializeField] RenderTexture backgroundTex;

    [SerializeField] Image uiMask;

    string inputDevice;
    bool gamePaused;
    GameObject pauseScreenInstance;
    PauseScreenData data;



    void  Start()
    {
    }

    void Update()
    {
         if (!gamePaused)
         {
            WaitForInputToPause();
         }
         else
         {
            GamePaused();
         }
    }

    void WaitForInputToPause()
    {
            string parentInput = GameData.GetData<ParentData>("Parent").tempInputDevice;
            string childInput = GameData.GetData<ChildData>("Child").tempInputDevice;

            if (Input.GetButtonDown(parentInput+"Start"))
            {
                inputDevice = parentInput;
                PauseGame();
            }
            else if (Input.GetButtonDown(childInput+"Start"))
            {
                inputDevice = childInput;
                PauseGame();
            } 
    }

    void PauseGame()
    {
        pauseScreenInstance = Instantiate(pauseScreenPrefab);
        pauseScreenInstance.transform.name = "PauseScreen_instance";
        pauseScreenInstance.transform.SetParent(parent);

        data = pauseScreenInstance.GetComponentInChildren<PauseScreenData>();
        data.unpauseButton.onClick.AddListener(()=>UnpauseGame()); 
        data.backToMainMenuButton.onClick.AddListener(()=>BackToMainMenu());

        Time.timeScale = 0;
        gamePaused = true;

        SetDepthOfField(true);
        SetMask(true);
        WorldSpaceUI.SetWorldUI(false);
        LastInputDevice.SetMouseCursorGlobally(true);
    }

    void SetMask(bool active)
    {   
        if (active)
            uiMask.color = new Color(0,0,0,0);
        else
            uiMask.color = Color.black;
    }

 

    void UnpauseGame()
    {   
        SetDepthOfField(false);
        SetMask(false);
        WorldSpaceUI.SetWorldUI(true);

        Destroy(pauseScreenInstance);
        gamePaused = false;
        Time.timeScale = 1;
        LastInputDevice.SetMouseCursorGlobally(false);
    }

    void SetDepthOfField(bool active)
    {
        DepthOfField depthOfField;
        VolumeProfile postProcessVolumeProfile = Camera.main.GetComponentInChildren<Volume>().profile;
        postProcessVolumeProfile.TryGet<DepthOfField>(out depthOfField);
        depthOfField.active = active;
    }


    void GamePaused()
    {
        if (Input.GetButtonDown(inputDevice+"Start")|Input.GetButtonDown("Start"))
            UnpauseGame();
    }

    void BackToMainMenu()
    {
        UnpauseGame();
        ScreenSwitcher.SwitchScreen(ScreenType.MainMenu);
    }

    void ToControlsScreen()
    {
        //LoadControlsScreenOnTop
    }

}

