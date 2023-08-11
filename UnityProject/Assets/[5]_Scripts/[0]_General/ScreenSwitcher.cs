using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum Screen
{
    Empty,MainMenu, ControllerSelect, CharacterSelect, GameScreen
}


public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] Screen startScreen;

    [Header ("Screen Prefabs")]

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controllerSelect;
    [SerializeField] GameObject characterSelect;
    [SerializeField] GameObject game;
    private static ScreenSwitcher instance;
    static Dictionary<Screen, GameObject> activeScreenDataBase = new Dictionary<Screen, GameObject>();

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
        instance.DeactivateAllScreens ();
        instance.ActivateScreen(screen);
    }

    public static void AddScreen(Screen screen)
    {
        instance.ActivateScreen(screen);
    }

    void ActivateScreen(Screen screen){
        
        GameObject screenByName  = GetScreenByName(screen);
        GameObject newScreen = Instantiate(screenByName);
        newScreen.name = screenByName.name+ " instance";
        activeScreenDataBase.TryAdd(screen,newScreen);
    }

    public static void DeactivateScreen(Screen screen)
    {
        activeScreenDataBase.TryGetValue(screen, out GameObject screenToDeactivate);
        
        if (screenToDeactivate!=null)
            Destroy(screenToDeactivate);

        activeScreenDataBase.Remove(screen);
    }

    void DeactivateAllScreens()
    {
        GameObject[] screensToDeactivate = activeScreenDataBase.Values.ToArray<GameObject>();

        for (int i= 0; i<screensToDeactivate.Length;i++)
        {
            if (screensToDeactivate[i]!=null)
                Destroy(screensToDeactivate[i]);
        }

        activeScreenDataBase.Clear();
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
