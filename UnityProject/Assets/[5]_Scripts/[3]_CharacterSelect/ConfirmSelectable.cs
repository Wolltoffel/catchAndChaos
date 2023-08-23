using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfirmSelectable : CustomSelectable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] Button left, right;
    Sprite leftDefault, rightDefault;

    string inputDeviceChild, inputDeviceParent;
    bool activeControllerChild, activeControllerParent;
    bool childConfirmed,parentConfirmed;
    bool parentDataSet, childDataSet;

    public void SetData(string inputDeviceChild, string inputDeviceParent, bool activeControllerChild, bool activeControllerParent)
    {
        this.inputDeviceChild = inputDeviceChild;
        this.inputDeviceParent = inputDeviceParent;
        this.activeControllerChild = activeControllerChild;
        this.activeControllerParent = activeControllerParent;
        childDataSet = true;
        parentDataSet  = true;
    }

    public void SetData(Characters characterToSetDataTo, string inputDevice, bool activeController)
    {
        if (characterToSetDataTo == Characters.Child)
        {
            inputDeviceChild = inputDevice;
            activeControllerChild  = activeController;
            childDataSet = true;
        }
        else if (characterToSetDataTo == Characters.Parent)
        {
            inputDeviceParent = inputDevice;
            activeControllerParent  = activeController;
            parentDataSet  = true;
        }
    }

    protected override void Awake() 
    {
        left.onClick.AddListener(()=>ConfirmBoth());
        right.onClick.AddListener(() => ConfirmBoth());

        leftDefault = left.GetComponent<Image>().sprite;
        rightDefault = right.GetComponent<Image>().sprite;
    }

    void Confirm (Characters characters)
    {
        if (characters == Characters.Child)
        {
            childConfirmed = true;
            left.GetComponent<Image>().sprite = left.spriteState.pressedSprite;
        }
        else if (characters == Characters.Parent)
        {
            parentConfirmed = true;
            right.GetComponent<Image>().sprite = right.spriteState.pressedSprite;
        }
    }

   void  Update()
   {
        WaitForConfirm();
   }
   void ConfirmBoth()
   {
        Confirm(Characters.Child);
        Confirm(Characters.Parent);
   }

   void WaitForConfirm()
   {
        if (activeControllerChild && Input.GetButtonDown(inputDeviceChild+"A") && childDataSet)
        {
            left.onClick.Invoke();
        }
        else if (activeControllerParent &&Input.GetButtonDown(inputDeviceParent+"A") && parentDataSet)
        {
            right.onClick.Invoke();
        }
   }



   public bool GetConfirmed (Characters characters)
   {
        if (characters == Characters.Child)
            return childConfirmed;
        else if (characters == Characters.Parent)
            return parentConfirmed;
        else
            throw new System.Exception ("No character set");
   }

   public bool GetFullyConfirmed()
   {
        if (childConfirmed && parentConfirmed)
            return true;
        return false;
   }


    public override void OnPointerEnter(PointerEventData eventData) 
    {
        left.GetComponent<Image>().sprite = left.spriteState.highlightedSprite;
        right.GetComponent<Image>().sprite = right.spriteState.highlightedSprite;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        left.GetComponent<Image>().sprite = leftDefault;
        right.GetComponent<Image>().sprite = rightDefault;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ConfirmBoth();
    }

}
