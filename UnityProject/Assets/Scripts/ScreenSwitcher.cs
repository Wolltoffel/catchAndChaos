using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Screen
{
    Empty,MainMenu, ControllerSelect, CharacterSelect, GameScreen
}


[CreateAssetMenu(fileName = "ScreenSwitcher", menuName = "Custom/ScreenSwitcher", order = 1)]
public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] Screen startScreen;

    [Header ("Screen Prefabs")]

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controllerSelect;
    [SerializeField] GameObject characterSelect;
    [SerializeField] GameObject game;


    private static ScreenSwitcher instance;
    static Screen activeScreen;
    static GameObject spawnedScreen;


    void Awake()
    {
        if (instance==null)
            instance = this;
        else if (instance!=this)
            Destroy(instance);

        SetStartScreen();
    }

    void OnDestroy()
    {
        if (instance==this)
            instance = null;
    }

    public void SetStartScreen()
    {   
        ActivateScreen(startScreen);
    }

    public static void SwitchScreen(Screen screen)
    {
        instance.DeactivateScreen (activeScreen);
        instance.ActivateScreen(screen);
        activeScreen = screen;
    }

    void ActivateScreen(Screen screen){
        
        GameObject screenByName  = GetScreenByName(screen);
        spawnedScreen = Instantiate(screenByName);
        spawnedScreen.name = screenByName.name+ " instance";
        activeScreen = screen;
    }

    void DeactivateScreen(Screen screen)
    {
        if (spawnedScreen!=null)
            Destroy(spawnedScreen);
        activeScreen = Screen.Empty;
        spawnedScreen = null;
    }

    GameObject GetScreenByName(Screen screen)
    {
        switch (screen)
        {
            case Screen.MainMenu:
                return mainMenu;
            case Screen.ControllerSelect:
                return controllerSelect;
            case Screen.CharacterSelect:
                return characterSelect;
            case Screen.GameScreen:
                return game;
        }

        throw new SystemException ($"No screen with the name {screen} existent.");
    }

}
