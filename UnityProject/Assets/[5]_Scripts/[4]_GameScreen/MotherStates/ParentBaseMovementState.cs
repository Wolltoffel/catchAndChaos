abstract class ParentBaseMovementState
{
  protected ParentData parentData;
  public ParentBaseMovementState(ParentData data)
  {
    parentData = data;
  }
  public abstract ParentBaseMovementState UpdateState();
}
