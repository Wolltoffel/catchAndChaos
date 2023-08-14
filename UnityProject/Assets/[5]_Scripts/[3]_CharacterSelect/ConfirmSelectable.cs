using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSelectable : CustomSelectable
{
   [SerializeField]Button confirmButton;

    [SerializeField]Sprite confirmLeftSprite, confirmRightSprite, bothConfirmedSprite;

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
        confirmButton.onClick.AddListener(()=>ConfirmBoth());
    }

   void  Update()
   {
        WaitForConfirm();
   }
   void ConfirmBoth()
   {
        childConfirmed = true;
        parentConfirmed = true;
   }

   void WaitForConfirm()
   {

            if (activeControllerChild && Input.GetButtonDown(inputDeviceChild+"A") && childDataSet)
            {
                childConfirmed = true;
            }
            else if (activeControllerParent &&Input.GetButtonDown(inputDeviceParent+"A") && parentDataSet)
            {
                parentConfirmed = true;
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


}
