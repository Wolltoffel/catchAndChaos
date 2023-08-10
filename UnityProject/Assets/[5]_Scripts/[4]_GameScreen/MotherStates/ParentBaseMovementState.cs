using UnityEngine;

abstract class ParentBaseMovementState : State
{
    protected ParentData parentData;
    public ParentBaseMovementState(ParentData data)
    {
    parentData = data;
    }

    public abstract ParentBaseMovementState UpdateState();

    protected void CheckDoorToggle(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Door", out interactable))
        {
            //show prompt

            if (Input.GetButton($"{inputDevice}B"))
            {
                //DoAnimation
                //Sound

                DoorSwitch toggle = interactable.GetComponent<DoorSwitch>();
                toggle.Toggle();
            }
        }
        else
        {
            //hideprompt
        }
    }
    protected void CheckPlushie(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            //show prompt
            if (Input.GetButton($"{inputDevice}X"))
            {

            }
        }
        else
        {
            //hideprompt
        }
    }
}
