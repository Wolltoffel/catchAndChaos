using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField]ScreenType screenToJumpTo;
    Button button;

    void  Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>GoBack());
    }

    void Update()
    {
        WaitForControllerInput();
    }

    void GoBack()
    {
        ScreenSwitcher.SwitchScreen(screenToJumpTo);
    }

    public void SetScreenScreenToJumpTo(ScreenType screenToJumpTo)
    {
        this.screenToJumpTo = screenToJumpTo;
    }

    void WaitForControllerInput()
    {
        if (Input.GetButtonDown("AB"))
        {
            GoBack();
        }
    }
}
