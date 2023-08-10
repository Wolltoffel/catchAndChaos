using UnityEngine;

class ParentCatch : ParentBaseMovementState
{
    private Coroutine catchCoroutine;
    private float time;
    private float catchDistance;
    private GameObject parent;
    private MovementScript movement;

    public ParentCatch(ParentData data, ParentBaseObjectState objectState) : base(data, objectState)
    {
        time = 0;
        parent = data.gameObject;
        movement = parent.GetComponent<MovementScript>();
        movement.DoCatch();
    }

    public override ParentBaseMovementState UpdateState()
    {
        if (movement.IsCoroutineDone)
        {
            return new ParentIdle(parentData,objectState);
        }

        return this;
    }
}
