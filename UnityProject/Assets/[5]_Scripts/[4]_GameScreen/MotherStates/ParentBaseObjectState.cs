using UnityEngine;

abstract class ParentBaseObjectState : State
{
    protected ParentData parentData;

    public ParentBaseObjectState(ParentData data)
    {
        parentData = data;
    }
    public abstract ParentBaseObjectState UpdateState(out bool hasBeenThrown);
    }
