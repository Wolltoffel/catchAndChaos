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

    protected Plushie CheckPlushiePickup(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            //show prompt

            if (Input.GetButton($"{inputDevice}X"))
            {
                //hideprompt

                Plushie plushie = interactable.GetComponent<Plushie>();

                return plushie;
            }
        }
        else
        {
            //hideprompt
        }
        return null;
    }

    protected Plushie CheckPlushieThrow(string inputDevice, Plushie plushie, out bool hasBeenThrown)
    {
        hasBeenThrown = false;
        if (Input.GetButton($"{inputDevice}X"))
        {
            plushie.ThrowPlushie();
            hasBeenThrown = true;
            return null;
        }
        return plushie;
    }

    protected bool CheckForPlushie(string inputDevice)
    {
        bool hasBeenThrown = false;
        if (parentData.plushie == null)
            parentData.plushie = CheckPlushiePickup(inputDevice);
        else
            parentData.plushie = CheckPlushieThrow(inputDevice, parentData.plushie, out hasBeenThrown);
        return hasBeenThrown;
    }

    protected bool GetMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;
    }
}
