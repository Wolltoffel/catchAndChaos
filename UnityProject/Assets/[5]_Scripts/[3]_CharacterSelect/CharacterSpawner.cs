using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner 
{   
    static GameObject childInstance, parentInstance;

    public CharacterSpawner(GameObject placeHolderParent, GameObject placeHolderChild)
    {
        childInstance = placeHolderChild;
        parentInstance = placeHolderParent;
    }

    public static void Initialize(GameObject placeHolderParent, GameObject placeHolderChild)
    {
        childInstance = placeHolderChild;
        parentInstance = placeHolderParent;
    }

    public static void UpdateCharacter(Characters characters)
    {
        if (childInstance == null)
        {
            Debug.Log ("chlild isntacen is null");
            childInstance = GameObject.Instantiate (new GameObject("ChildSpawnPosition"));
        }
            
        if (parentInstance == null)
            parentInstance = GameObject.Instantiate (new GameObject("ParentSpawnPosition"));

        GameObject instance;
        GameObject oldInstance;
        Transform instanceParent;

        if (characters == Characters.Child)
        {
            instance = childInstance;
        }
        else
        {
            instance = parentInstance;
        }

        oldInstance = instance;
        instanceParent = instance.transform.parent;

        CharacterInstantiator.InstantiateCharacter(characters, out instance,instance.transform.position);
        instance.transform.rotation = oldInstance.transform.rotation;
        instance.transform.parent = instanceParent;
        instance.name =  instance.name+ "_Spawned";

        if (characters == Characters.Child)
        {
            childInstance = instance;
        }
        else
        {
            parentInstance = instance;
        }
               
        GameObject.Destroy (oldInstance);
        
    }
}
