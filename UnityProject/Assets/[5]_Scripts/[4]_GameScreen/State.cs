using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public GameObject gameObject;
    public bool InteractableInRange(string interactableTag, out GameObject interactableObject) 
    {
        Interactable interactable = GameData.GetData<InteractableHolder>("InteractableHolder").GetInteractable(interactableTag);
        float range = interactable.range;
        List<GameObject> interactableObjects = interactable.objects;
        Vector3 characterPosition = gameObject.transform.position;

        for (int i= 0; i<interactableObjects.Count;i++)
        {
            if (Vector3.Distance (interactableObjects[i].transform.position,characterPosition)>range)
            {
                interactableObject = interactableObjects[i];
                return true;
            }
        }  
        interactableObject = null;
        return false;
    }
}
