class ParentObject : ParentBaseObjectState
{
    public ParentObject(ParentData data) : base(data) {}

    public override ParentBaseObjectState UpdateState()
    {
        return this;
    }
}
