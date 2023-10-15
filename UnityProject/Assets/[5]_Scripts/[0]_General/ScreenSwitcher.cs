using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum ScreenType
{
    Empty,MainMenu,CharacterInputSelect, GameScreen, ScoreInterim, EndScreen
}


public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] ScreenType startScreen;


    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreen;

    [Header ("Screen Prefabs")]

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject characterInputSelect;
    [SerializeField] GameObject game;
    [SerializeField] GameObject scoreInterim;
    [SerializeField] GameObject endScreen;
    private static ScreenSwitcher instance;
    static Dictionary<ScreenType, GameObject> activeScreenDataBase = new Dictionary<ScreenType, GameObject>();

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

    public static void SwitchScreen(ScreenType screen, bool activateLoad = true)
    {
        instance.StartCoroutine(instance._SwitchScreen(screen, activateLoad));
    }

    public IEnumerator _SwitchScreen(ScreenType screen ,bool activateLoad)
    {
        if (loadingScreen != null && activateLoad)
        {
            LoadingScreenManager manager = Instantiate(loadingScreen).GetComponent<LoadingScreenManager>();
            yield return new WaitForSeconds(manager.loadTime-2);
        }
        instance.DeactivateAllScreens();
        instance.ActivateScreen(screen);
    }

    public static void OutsourceCoroutine(IEnumerator enumerator)
    {
        instance.StartCoroutine(enumerator);
    }

    public static void AddScreen(ScreenType screen)
    {
        instance.ActivateScreen(screen);
    }

    void ActivateScreen(ScreenType screen){
        
        GameObject screenByName  = GetScreenByName(screen);
        GameObject newScreen = Instantiate(screenByName);
        newScreen.name = screenByName.name+ "_instance";
        activeScreenDataBase.TryAdd(screen,newScreen);
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
        }

        throw new SystemException ($"No screen with the name {screen} existent.");
    } 

}
