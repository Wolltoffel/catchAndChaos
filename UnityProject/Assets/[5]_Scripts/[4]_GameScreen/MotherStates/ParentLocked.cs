class ParentLocked : ParentBaseMovementState
{
    public ParentLocked(ParentData data) : base(data)
    {
        data.plushie.ThrowPlushie();
    }

    public override ParentBaseMovementState UpdateState()
    {
        if (parentData.plushie.IsThrowDone)
        {
            parentData.plushie = null;
            return new ParentIdle(parentData);
        }
        return this;
    }
}
