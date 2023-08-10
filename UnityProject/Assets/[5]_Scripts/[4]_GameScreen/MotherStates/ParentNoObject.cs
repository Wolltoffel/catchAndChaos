class ParentNoObject : ParentBaseObjectState
{
    public ParentNoObject(ParentData data) : base(data) {}

    public override ParentBaseObjectState UpdateState()
    {
        return this;
    }
}
