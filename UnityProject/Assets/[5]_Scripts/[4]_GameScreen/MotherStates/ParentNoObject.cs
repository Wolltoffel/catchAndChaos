using UnityEngine;

class ParentNoObject : ParentBaseObjectState
{
    private GameObject prompt;

    public ParentNoObject(ParentData data) : base(data){}

    public override ParentBaseObjectState UpdateState(out bool hasBeenThrown)
    {
        hasBeenThrown = false;

        GameObject interactable;
        if (InteractableInRange("Plushie", out interactable))
        {
            //show prompt

            if (Input.GetButton($"{parentData.tempInputDevice}X"))
            {
                //hideprompt

                Plushie plushie = interactable.GetComponent<Plushie>();
                
                return new ParentObject(parentData, plushie);
            }
        }
        else
        {
            if (prompt != null)
            {
                //hideprompt
            }
        }
        return this;
    }
}
