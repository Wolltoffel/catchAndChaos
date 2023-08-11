using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstantiator : MonoBehaviour
{
    [SerializeField] private float initialScale = 20;

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
            character = child = Instantiate(characterData.characterAssets.GetContainer().prefab);
        }
        else
        {
            Destroy(parent);
            characterData = parentData;
            character = parent = Instantiate(characterData.characterAssets.GetContainer().prefab);

        }
        
        character.transform.position = position;
        character.transform.localScale = Vector3.one * _initialScale;
    }

    public static GameObject GetActiveCharacter(Characters character)
    {
        if (character == Characters.Child)
        {
            return child;
        }
        return parent;
    }
}