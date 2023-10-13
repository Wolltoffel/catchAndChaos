using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class State 
{
    protected GameObject gameObject;
    protected static int currentObjectHash;

    protected bool InteractableInRange(Characters character, out GameObject interactableObject , out string tag) 
    {
        interactableObject = GameData.GetData<InteractableContainer>("InteractableContainer").GetClosestInteractableForCharacter(character,gameObject.transform.position, out tag);

        //Check if an object is inbetween target object
        if (interactableObject != null)
        {
            bool hit = Physics.Linecast(gameObject.transform.position + Vector3.up * 1, interactableObject.transform.position, out RaycastHit hitInfo, LayerMask.GetMask("Walls"));
            if (!hit)
                return true;
            else if (hitInfo.collider != null && !(hitInfo.collider.gameObject.tag == tag || hitInfo.collider.gameObject.tag == "Ignore"))
                return false;
        }
        else
            return false;

        return true;
    }
}
