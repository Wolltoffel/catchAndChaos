using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Screen
{
    MainMenu, ControllerSelect, CharacterSelect, GameScreen
}

[CreateAssetMenu(fileName = "Data", menuName = "Custom/ScreenSwitcher", order = 1)]
public class ScreenSwitcher : ScriptableObject
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controllerSelect;
    [SerializeField] GameObject characterSelect;
    [SerializeField] GameObject game;


    public void SwitchScreen(Screen screen)
    {
        DeactivateAllScreens();
        ActivateScreen(screen);
    }

    void ActivateScreen(Screen screen){
        Instantiate(GetScreenByName(screen));
    }

    void DeactivateScreen(Screen screen)
    {
        GameObject screenToBeDestroyed = GetScreenByName(screen);
        if (screenToBeDestroyed!=null)
            Destroy(screenToBeDestroyed);
    }

    void DeactivateAllScreens() {
        
        int numberOfScreens = Enum.GetValues(typeof(Screen)).Length; //Get Number of Elements inside Enum

        for (int i = 0; i<numberOfScreens;i++)
        {
            Screen screenToDeactivate = (Screen)Enum.GetValues(typeof(Screen)).GetValue(i);
            DeactivateScreen (screenToDeactivate);
        }
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

        Debug.Log ("No screen by this enum found");
        return null;
    }

}
