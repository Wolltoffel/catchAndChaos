using UnityEngine;

abstract class ParentBaseMovementState : State
{
    protected ParentData parentData;
    protected ParentBaseObjectState objectState;

    public ParentBaseMovementState(ParentData data, ParentBaseObjectState objectState)
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
}
