using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    private TextMeshProUGUI textField;

    void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(int i)
    {
        if (i >= 60)
        {
            int minutes = i / 60;
            int seconds = i % 60;
            textField.text = $"{minutes}:{seconds:D2}";
        }
        else
        {
            textField.text = $"{i} seconds";
        }
    }
}

