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

            if (Input.GetButtonDown($"{inputDevice}B"))
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

    protected Plushie GetPlushieIfInRange(string inputDevice)
    {
        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            //show prompt

            if (Input.GetButtonDown($"{inputDevice}X"))
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

    protected bool CheckForPlushieAction(string inputDevice)
    {
        if (parentData.plushie == null)
        {
            parentData.plushie = GetPlushieIfInRange(inputDevice);
            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                if (parentData.plushie != null)
                {
                    parentData.plushie.AttachToTarget(gameObject.transform);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown($"{inputDevice}X"))
            {
                return true;
            }
        }

        return false;
    }

    protected bool GetMovement(string inputDevice, out float xAxis, out float yAxis)
    {
        xAxis = Input.GetAxis(inputDevice + " Horizontal");
        yAxis = Input.GetAxis(inputDevice + " Vertical");

        return xAxis != 0 || yAxis != 0;
    }
}
