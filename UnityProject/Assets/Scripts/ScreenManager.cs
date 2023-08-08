using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    IScreenState screenState;

    void SwitchState()
    {
        screenState = screenState.UpdateScreen();
    }
}

interface IScreenState
{
    public IScreenState UpdateScreen();
}


class MainMenu: IScreenState
{
    public IScreenState UpdateScreen(){
        //Inputmanager.AddKeyInput (StartGame, KeyCode. Enter, 0)

        if (Input.GetKeyDown(KeyCode.Space))
            return new ControllerSelect();
        return this;
    }

    public IScreenState StartGame(){
        //Switch Screen
        //Spawn Player ...
        return new ControllerSelect();
    }
}

class ControllerSelect: IScreenState
{
    public IScreenState UpdateScreen(){
        return this;
    }
}

class CharacterSelect: IScreenState
{
    public IScreenState UpdateScreen(){
        return this;
    }
}

class Game: IScreenState
{
    public IScreenState UpdateScreen(){
        return this;
    }
}

