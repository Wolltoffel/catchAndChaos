using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControllerUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI child;
    [SerializeField]TextMeshProUGUI parent;

    public void ControllerSet(string character, string inputDevice)
    {
        Debug.Log ($"Player {character}: Input is {inputDevice}");

        if (character=="Child")
            child.text = $"Child Input is {inputDevice}";
        else if (character=="Parent")
            child.text = $"Parent Input is {inputDevice}";
        else       
            throw new System.Exception("There's no other text then child and parent");

    }
}
