using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ReadyButton: IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    Button leftDefault, rightDefault, leftPressed, rightPressed;
    bool childConfirmed,parentConfirmed;

    public void Confirm (Characters characters)
    {
        if (characters == Characters.Child)
        {
            childConfirmed = true;
        }
        else if (characters == Characters.Parent)
        {
            parentConfirmed = true;
        }
    }

   void ConfirmBoth()
   {
        Confirm(Characters.Child);
        Confirm(Characters.Parent);
   }

    public void OnPointerDown(PointerEventData pointerEventData){}
    public void OnPointerEnter(PointerEventData pointerEventData){}
    public void OnPointerExit (PointerEventData pointerEventData){}

}
