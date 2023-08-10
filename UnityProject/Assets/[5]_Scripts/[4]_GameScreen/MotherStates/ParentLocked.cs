class ParentLocked : ParentBaseMovementState
{
    public ParentLocked(ParentData data) : base(data) { }

    public override ParentBaseMovementState UpdateState()
    {
        return this;
    }
}
