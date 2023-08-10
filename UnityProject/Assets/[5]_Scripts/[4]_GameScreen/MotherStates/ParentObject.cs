using UnityEngine;

class ParentObject : ParentBaseObjectState
{
    private Plushie plushie;

    public ParentObject(ParentData data, Plushie plushie) : base(data)
    {
        this.plushie = plushie;
        plushie.AttachToTarget(parentData.handTransform);
    }

    public override ParentBaseObjectState UpdateState(out bool hasBeenThrown)
    {
        hasBeenThrown = false;

        if (Input.GetButton($"{parentData.tempInputDevice}X"))
        {
            plushie.ThrowPlushie();
            hasBeenThrown = true;
            return new ParentNoObject(parentData);
        }

        return this;
    }
}
