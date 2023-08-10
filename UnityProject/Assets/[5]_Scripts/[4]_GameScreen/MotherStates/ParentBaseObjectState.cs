abstract class ParentBaseObjectState
{
    protected ParentData parentData;
    public ParentBaseObjectState(ParentData data)
    {
        parentData = data;
    }
    public abstract ParentBaseObjectState UpdateState();
}