using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected GameObject gameObject;

    protected bool InteractableInRange(string interactableTag, out GameObject interactableObject) 
    {
        interactableObject = GameData.GetData<InteractableContainer>("InteractableContainer").GetClosestInteractable(interactableTag,gameObject.transform.position);

        //Check if an object is inbetween target object
        if (interactableObject != null)
        {
            bool hit = Physics.Linecast(gameObject.transform.position+ Vector3.up*1, interactableObject.transform.position, out RaycastHit hitInfo);
            if (hitInfo.collider !=null && hitInfo.collider.gameObject.tag != interactableTag)
                return false;
        }

        if (interactableObject == null)
            return false;
        return true;
    }
}
