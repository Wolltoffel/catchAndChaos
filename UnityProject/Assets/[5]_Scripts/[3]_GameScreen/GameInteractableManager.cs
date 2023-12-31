using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableContainer : Object
{
    private InteractableCategory[] interactables;

    public InteractableContainer(InteractablePresets[] interactableTags)
    {
        interactables = new InteractableCategory[interactableTags.Length];

        for (int i = 0; i < interactableTags.Length; i++)
        {
            interactables[i] = new InteractableCategory(interactableTags[i].tag, interactableTags[i].interactionRange, interactableTags[i].character);
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

    private InteractableCategory[] GetInteractableCategoriesForCharacter(Characters character)
    {
        List<InteractableCategory> interactablesForCharacter = new List<InteractableCategory>();
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].character == character)
                interactablesForCharacter.Add(interactables[i]);
        }
        return interactablesForCharacter.ToArray();
    }

    public GameObject GetClosestInteractableForCharacter(Characters character, Vector3 position, out string category)
    {
        InteractableCategory[] interactables = GetInteractableCategoriesForCharacter(character);

        GameObject closestObject = null; 
        float currentDistance = -1;
        category = "";

        for (int i = 0; i < interactables.Length; i++)
        {
            float distance;
            GameObject obj = GetClosestInteractableInCategory(interactables[i], position, out distance);
            if (obj != null && (distance < currentDistance || currentDistance < 0))
            {
                currentDistance = distance;
                closestObject = obj;
                category = interactables[i].tag;
                //Debug.Log($"{obj.name} distance: {distance}");
            }
        }

        return closestObject;
    }

    //public GameObject GetClosestInteractableInCategory(string tag, Vector3 position)
    //{
    //    InteractableCategory interactable = GetInteractableCategory(tag);
    //    return GetClosestInteractableInCategory(interactable,position, out float t);
    //}

    private GameObject GetClosestInteractableInCategory(InteractableCategory interactable, Vector3 position, out float distance)
    {
        if (interactable.objects == null || interactable.objects.Count == 0)
        {
            distance = 0;
            return null;
        }

        position.y = 0;

        GameObject closestObject = interactable.objects[0];
        distance = Vector3.Distance(new(closestObject.transform.position.x,position.y, closestObject.transform.position.z), position);

        for (int i = 1; i < interactable.objects.Count; i++)
        {
            Transform currentObj = interactable.objects[i].transform;
            Vector3 currentPosition = currentObj.position;
            currentPosition.y = 0;
            float newDistance = Vector3.Distance(currentPosition, position);
            closestObject = newDistance < distance ? interactable.objects[i] : closestObject;
            distance = newDistance < distance ? newDistance : distance;
        }

        return distance > interactable.interactRange ? null : closestObject;
    }

    public void AddObjectsWithTag(string tag)
    {
        GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag(tag);
    }
}

public class InteractableCategory
{
    public readonly Characters character;
    public readonly string tag;
    public readonly float interactRange;
    public List<GameObject> objects = new List<GameObject>();

    public InteractableCategory(string tag, float interactRange, Characters character)
    {
        this.tag = tag;
        this.interactRange = interactRange;
        this.character = character;
        this.objects = new List<GameObject>();
    }
}

[System.Serializable]
public struct InteractablePresets
{
    public Characters character;
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
            objectsWithTag = RemoveSimilars(objectsWithTag);
            if (objectsWithTag.Length == 0 || objectsWithTag == null)
                Debug.Log($"No Objects Found \nTag: {tag}");
            else
            {
                allInteractables.RemoveAllObjectsFromCategory(tag);
                allInteractables.AddObjectsToCategory(tag, objectsWithTag);
            }
        }
        GameData.SetData(allInteractables, "InteractableContainer");
    }

    private GameObject[] RemoveSimilars(GameObject[] objects)
    {
        List<GameObject> list = new List<GameObject>();

        list.AddRange(objects);

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i+1; j < list.Count; j++)
            {
                if (list[i].transform.position == list[j].transform.position)
                {
                    var obj = FindChild(list[i], list[j]);
                    if (obj != null)
                    {
                        list.Remove(obj);
                        break;
                    }
                }
            }
        }
        return list.ToArray();
    }

    public static GameObject FindChild(GameObject similar1, GameObject similar2)
    {
        Transform current = similar1.transform;

        while (current != null)
        {
            if (current.gameObject == similar2)
            {
                return similar1; 
            }
            current = current.parent;
        }

        current = similar2.transform;

        while (current != null)
        {
            if (current.gameObject == similar1)
            {
                return similar2;
            }
            current = current.parent;
        }

        return null;
    }
}
