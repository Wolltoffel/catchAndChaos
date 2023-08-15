using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterInstantiator : MonoBehaviour
{
    [SerializeField] private float initialScale = 4.5f;

    private static float _initialScale;
    private static ParentData parentData;
    private static ChildData childData;

    private static GameObject parent;
    private static GameObject child;

    private void Awake()
    {
        _initialScale = initialScale;
        parentData = GameData.GetData<ParentData>("Parent");
        childData = GameData.GetData<ChildData>("Child");
    }

    public static void InstantiateCharacter(Characters characterType, out GameObject character, Vector3 position)
    {
        PlayerData characterData;
        if (characterType == Characters.Child)
        {
            Destroy(child);
            characterData = childData;
            character = child = Instantiate(characterData.characterAssets.prefab);
        }
        else
        {
            Destroy(parent);
            characterData = parentData;
            character = parent = Instantiate(characterData.characterAssets.prefab);

        }

        character.transform.position = position;
        character.transform.localScale = Vector3.one * _initialScale;
    }

    public static void InstantiateCharacter(Characters characterType, out GameObject character, Transform parentOfCharacter, bool addParentConstraint = false)
    {
        InstantiateCharacter(characterType, out character, parentOfCharacter.position);
        
        if (addParentConstraint)
        {
            ParentConstraint parentConstraint = character.AddComponent<ParentConstraint>();
            parentConstraint.constraintActive = true;
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = parentOfCharacter.transform;
            source.weight = 1f;
            parentConstraint.AddSource (source);
        }

        //character.GetComponent<Animator>().enabled = false;
        character.transform.parent = parentOfCharacter;
        character.transform.localRotation = Quaternion.identity;
        character.transform.localPosition = Vector3.zero;
        //character.GetComponent<Animator>().enabled = true;
    }

     

    public static GameObject GetActiveCharacter(Characters character)
    {
        if (character == Characters.Child)
        {
            return child;
        }
        return parent;
    }

    public static void ReplaceCharacter (Characters characterType, out GameObject character, bool addConstraint)
    {
        Transform parent  = GetActiveCharacter(characterType).transform.parent;
        InstantiateCharacter(characterType, out character,parent, addConstraint);
        GetActiveCharacter(characterType).transform.parent = parent;
    }
}