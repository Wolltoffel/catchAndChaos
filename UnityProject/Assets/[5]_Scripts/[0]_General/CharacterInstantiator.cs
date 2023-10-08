using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum Characters{
    Child, Parent
}

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
            character.transform.localScale = Vector3.one * _initialScale / 1.4f;
        }
        else
        {
            Destroy(parent);
            characterData = parentData;
            character = parent = Instantiate(characterData.characterAssets.prefab);
            character.transform.localScale = Vector3.one * _initialScale;

        }

        character.transform.position = position;
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

    public static void SetActiveCharacter(Characters characterType, GameObject character)
    {
        if (characterType==Characters.Child)
            child = character;
        else
            parent = character;
    }

    public static void ReplaceCharacter (Characters characterType, out GameObject character, bool addConstraint)
    {
        GameObject activeCharacter = GetActiveCharacter(characterType);
        Transform parent  = activeCharacter.transform.parent;
        
        //Copy the animator data
        Animator activeAnimator = activeCharacter.GetComponent<Animator>();
        AnimatorStateInfo animatorStateInfo = activeAnimator.GetCurrentAnimatorStateInfo(0);
        int index;
        string parameterName="";
        if (characterType == Characters.Child)
            parameterName = "ChildIndex";
        else
            parameterName = "MomIndex";
        
        index = activeAnimator.GetInteger(parameterName);


        InstantiateCharacter(characterType, out character,parent, addConstraint);
        GetActiveCharacter(characterType).transform.parent = parent;

        //Apply animator data
        activeAnimator = character.GetComponent<Animator>();
        activeAnimator.SetInteger(parameterName,index);
        activeAnimator.Play(animatorStateInfo.fullPathHash, 0, animatorStateInfo.normalizedTime);
    }

    public static void AddCharacter(Characters characterType, out GameObject character,Transform parentOfCharacter,Vector3 position,int index,bool addParentConstraint=false)
    {
        PlayerData characterData;
        if (characterType == Characters.Child)
        {
            characterData = childData;
            character = child = Instantiate(characterData.characterAssets.GetCharacterAssetItemAt(index).characterPrefab);
            character.transform.localScale = Vector3.one * _initialScale / 1.4f;
        }
        else
        {
            characterData = parentData;
            character = parent = Instantiate(characterData.characterAssets.prefab);
            character.transform.localScale = Vector3.one * _initialScale;
        }

        if (addParentConstraint)
        {
            ParentConstraint parentConstraint = character.AddComponent<ParentConstraint>();
            parentConstraint.constraintActive = true;
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = parentOfCharacter.transform;
            source.weight = 1f;
            parentConstraint.AddSource (source);
        }

        character.transform.position = position;
    }
}