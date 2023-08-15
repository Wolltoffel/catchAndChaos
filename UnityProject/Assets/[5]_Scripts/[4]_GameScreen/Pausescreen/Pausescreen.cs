using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Pausescreen : MonoBehaviour
{
    [SerializeField] GameObject pauseScreenPrefab;

    string inputDevice;
    bool gamePaused;
    GameObject pauseScreenInstance;
    PauseScreenData data;

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
            Debug.Log ("Wait for input to pause");
            
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

        data = pauseScreenInstance.GetComponentInChildren<PauseScreenData>();
        data.unpauseButton.onClick.AddListener(()=>UnpauseGame()); 
        data.backToMainMenuButton.onClick.AddListener(()=>BackToMainMenu());

        pauseScreenInstance.transform.name = "PauseScreen_instance";
        Time.timeScale = 0;
        gamePaused = true;
        //StartCoroutine(SetSelectedObjectLater());
    }

    void UnpauseGame()
    {
        Debug.Log ("UnpauseGame");
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
        ScreenSwitcher.SwitchScreen(ScreenType.MainMenu);
    }

    void ToControlsScreen()
    {
        //LoadControlsScreenOnTop
    }

}

