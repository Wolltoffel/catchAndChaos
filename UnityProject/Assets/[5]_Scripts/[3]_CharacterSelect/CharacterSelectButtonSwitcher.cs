using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButtonSwitcher : ButtonSwitcher
{
  [SerializeField] public ButtonSwitchData prev, next;

    void Awake() 
    {
        buttons.Add (prev);
        buttons.Add(next);
    }

    Button GetActiveSelector(Bumper bumper)
    {
        if (bumper == Bumper.Left)
            return prev.GetActiveButton();
        else if (bumper == Bumper.Right)
            return next.GetActiveButton();

        throw new System.Exception ("No such button existent");
    }

}
