using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected GameObject gameObject;

    protected bool InteractableInRange(string interactableTag, out GameObject interactableObject) 
    {
        interactableObject = GameData.GetData<InteractableContainer>("InteractableContainer").GetClosestInteractable(interactableTag,gameObject.transform.position);
        if (interactableObject == null)
            return false;
        return true;
    }
}
