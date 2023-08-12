using UnityEngine;

class ParentCatch : ParentBaseMovementState
{
    private Coroutine catchCoroutine;
    private float time;
    private float catchDistance;
    private GameObject parent;
    private MovementScript movement;

    public ParentCatch(ParentData data, MovementScript movement) : base(data)
    {
        time = 0;
        parent = CharacterInstantiator.GetActiveCharacter(Characters.Parent);
        //movement = parent.GetComponent<MovementScript>();
        this.movement = movement;
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
