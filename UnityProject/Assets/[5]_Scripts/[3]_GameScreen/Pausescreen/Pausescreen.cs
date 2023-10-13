using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Pausescreen : MonoBehaviour
{
    [SerializeField] GameObject pauseScreenPrefab;
    [SerializeField] Transform parent;

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
        //StartCoroutine(SetSelectedObjectLater());
    }

    void UnpauseGame()
    {
        Destroy(pauseScreenInstance);
        gamePaused = false;
        Time.timeScale = 1;
    }

    void GamePaused()
    {
        if (Input.GetButtonDown(inputDevice+"Start"))
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

