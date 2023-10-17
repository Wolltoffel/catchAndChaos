using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum ScreenType
{
    Empty,MainMenu,CharacterInputSelect, GameScreen, ScoreInterim, EndScreen, ControlSchemeScreen
}


public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] ScreenType startScreen;


    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject loadingScreenCompact;

    [Header ("Screen Prefabs")]

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject characterInputSelect;
    [SerializeField] GameObject game;
    [SerializeField] GameObject scoreInterim;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject controlScheme;
    private static ScreenSwitcher instance;
    static Dictionary<ScreenType, GameObject> activeScreenDataBase = new Dictionary<ScreenType, GameObject>();

    public static GameObject currentScreen;
    public static float lastTransitionTime = 0;

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
        currentScreen = ActivateScreen(startScreen);
    }

    public static void SwitchScreen(ScreenType screen, LoadingScreenType type = LoadingScreenType.Compact)
    {
        instance.StartCoroutine(instance._SwitchScreen(screen, type));
    }

    public IEnumerator _SwitchScreen(ScreenType screen, LoadingScreenType type)
    {
        if (loadingScreen != null)
        {
            switch (type)
            {
                case LoadingScreenType.Normal:
                    UIAnimationLengthManager manager = Instantiate(loadingScreen).GetComponent<UIAnimationLengthManager>();
                    yield return new WaitForSeconds(manager.animationLength / 2);
                    lastTransitionTime = manager.animationLength / 2;
                    break;
                case LoadingScreenType.Compact:
                    manager = Instantiate(loadingScreenCompact).GetComponent<UIAnimationLengthManager>();
                    yield return new WaitForSeconds(manager.animationLength / 2);
                    lastTransitionTime = manager.animationLength / 2;
                    break;
                case LoadingScreenType.Off:
                    lastTransitionTime = 0;
                    break;
            }
        }
        instance.DeactivateAllScreens();
        currentScreen = instance.ActivateScreen(screen);
    }

    public static void OutsourceCoroutine(IEnumerator enumerator)
    {
        instance.StartCoroutine(enumerator);
    }

    public static void AddScreen(ScreenType screen)
    {
        instance.ActivateScreen(screen);
    }

    public static void AddScreen(ScreenType screen, out GameObject screenInstance)
    {
        screenInstance = instance.ActivateScreen(screen);
    }

    GameObject ActivateScreen(ScreenType screen){
        
        GameObject screenByName  = GetScreenByName(screen);
        GameObject newScreen = Instantiate(screenByName);
        newScreen.name = screenByName.name+ "_instance";
        activeScreenDataBase.TryAdd(screen, newScreen);
        return newScreen;
    }

    public static void DeactivateScreen(ScreenType screen)
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

    GameObject GetScreenByName(ScreenType screen)
    {
        switch (screen)
        {
            case ScreenType.MainMenu:
                return mainMenu;
            case ScreenType.CharacterInputSelect:
                return characterInputSelect;
            case ScreenType.GameScreen:
                return game;
            case ScreenType.ScoreInterim:
                return scoreInterim;
            case ScreenType.EndScreen:
                return endScreen;
            case ScreenType.ControlSchemeScreen:
                return controlScheme;
        }

        throw new SystemException ($"No screen with the name {screen} existent.");
    }

    public enum LoadingScreenType
    {
        Normal,
        Compact,
        Off
    }

}
