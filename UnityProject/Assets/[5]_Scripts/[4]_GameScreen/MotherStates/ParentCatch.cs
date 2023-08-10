using UnityEngine;

class ParentCatch : ParentBaseMovementState
{
    private Coroutine catchCoroutine;
    private float time;
    private float catchDistance;
    private GameObject parent;
    private MovementScript movement;

    public ParentCatch(ParentData data) : base(data)
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
            return new ParentIdle(parentData);
        }

        return this;
    }
}
