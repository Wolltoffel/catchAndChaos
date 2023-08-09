using System;
using TMPro;
using UnityEngine;

public class SlideCooldownUI : MonoBehaviour
{
    private TextMeshProUGUI textField;

    void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(float i)
    {
        textField.text = $"{i}";
    }
}

