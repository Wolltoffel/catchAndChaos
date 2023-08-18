using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSelectUIManager : MonoBehaviour
{
    [SerializeField] GameObject childInput, parentInput;

    public void HideUI(Characters characters)
    {
        if (characters == Characters.Child)
            childInput.SetActive(false);
        else
            parentInput.SetActive(false);
    }
}
