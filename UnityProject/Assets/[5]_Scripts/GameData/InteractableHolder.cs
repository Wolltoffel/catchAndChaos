using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interactable
{
    public string tag;
    public float range;
    [HideInInspector]public List<GameObject> objects;
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/InteractableHolder")]
public class InteractableHolder : StaticData{


    [SerializeField]Interactable[] interactables ;
    GameObject map;

    public void ImportCurrentObjectWithTag()
    {
        for (int i = 0; i<interactables.Length; i++)
        {
            AddObjectsWithTag (interactables[i]);
        }
    }

    void AddObjectsWithTag(Interactable interactable)
    {
        if (map == null)
            throw new System.Exception ("Missing map reference");
        else
        {
            Transform[] transforms = map.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i<transforms.Length;i++)
            {
                if (transforms[i].CompareTag(interactable.tag))
                {
                    interactable.objects.Add (transforms[i].gameObject);
                }
            }
        }      
    }

    void AddGameObjects(GameObject gameobject, Interactable interactable)
    {
        if (!gameobject.transform.CompareTag(interactable.tag))
            throw new System.Exception ("GameObject does not match interactable's tag");
        else
            interactable.objects.Add (gameobject);
    }

    public Interactable GetInteractable(string tag)
    {  
        for (int i=0; i<interactables.Length; i++)
        {
            if (interactables[i].tag == tag)
                return interactables[i];
        }
        throw new System.Exception ("No interactable with this tag existing");
    }
    
}
