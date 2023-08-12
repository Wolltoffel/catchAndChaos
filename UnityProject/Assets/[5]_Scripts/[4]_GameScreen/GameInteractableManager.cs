using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : Object
{
    private InteractableCategory[] interactables;

    public InteractableContainer(InteractablePresets[] interactableTags)
    {
        interactables = new InteractableCategory[interactableTags.Length];

        for (int i = 0; i < interactableTags.Length; i++)
        {
            interactables[i] = new InteractableCategory(interactableTags[i].tag, interactableTags[i].interactionRange);
        }
    }

    public void AddObjectsToCategory(string tag, GameObject[] objects)
    {
        InteractableCategory category = GetInteractableCategory(tag);
        category.objects.AddRange(objects); 
    }

    public void AddObjectToCategory(string tag, GameObject obj)
    {
        InteractableCategory category = GetInteractableCategory(tag);
        category.objects.Add(obj);
    }

    public void RemoveAllObjectsFromCategory(string tag)
    {
        InteractableCategory category = GetInteractableCategory(tag);
        category.objects.Clear();
    }

    public void RemoveObjectFromCategory(string tag, GameObject obj)
    {
        InteractableCategory category = GetInteractableCategory(tag);
        if (category.objects.Contains(obj))
            category.objects.Remove(obj);
        else
            Debug.Log($"Object not in category. \nObj: {obj.name}\nCategory{tag}");
    }

    public InteractableCategory GetInteractableCategory(string tag)
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].tag == tag)
                return interactables[i];
        }
        throw new System.Exception($"Interactable Category not.\nTag \"{tag}\"");
    }

    public GameObject GetClosestInteractable(string tag, Vector3 position)
    {
        InteractableCategory interactable = GetInteractableCategory(tag);

        if (interactable.objects == null || interactable.objects.Count == 0)
        {
            return null;
        }

        GameObject closestObject = interactable.objects[0];
        float currentDistance = Vector3.Distance(closestObject.transform.position, position);

        for (int i = 1; i < interactable.objects.Count; i++)
        {
            Transform currentObj = interactable.objects[i].transform;
            closestObject = Vector3.Distance(currentObj.position, position) < currentDistance ? interactable.objects[i] : closestObject;
        }

        return currentDistance > interactable.interactRange ? null : closestObject;
    }

    public void AddObjectsWithTag(string tag)
    {
        GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag(tag);
    }
}

public class InteractableCategory
{
    public readonly string tag;
    public readonly float interactRange;
    public List<GameObject> objects = new List<GameObject>();

    public InteractableCategory(string tag, float interactRange)
    {
        this.tag = tag;
        this.interactRange = interactRange;
        this.objects = new List<GameObject>();
    }
}

[System.Serializable]
public struct InteractablePresets
{
    public string tag;
    public float interactionRange;
}

public class GameInteractableManager : MonoBehaviour
{
    [SerializeField] private InteractablePresets[] interactableTags;

    private InteractableContainer allInteractables;

    public void LoadInteractablesIntoDatabase()
    {
        allInteractables = new InteractableContainer(interactableTags);
        for (int i = 0; i < interactableTags.Length; i++)
        {
            string tag = interactableTags[i].tag;
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            if (objectsWithTag.Length == 0 || objectsWithTag == null)
                Debug.Log("No Objects Found");
            else
                allInteractables.AddObjectsToCategory(tag,objectsWithTag);
        }
        GameData.SetData(allInteractables, "InteractableContainer");
    }
}
