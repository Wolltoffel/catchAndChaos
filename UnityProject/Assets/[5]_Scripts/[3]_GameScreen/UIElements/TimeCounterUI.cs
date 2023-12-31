using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeCounterUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;

    void Awake()
    {
        if (textField == null)
        {
            textField = GetComponent<TextMeshProUGUI>();
            if (textField == null)
            {
                Destroy(this);
            }
        }
    }

    public void UpdateUI(int i)
    {
        i = Mathf.Max(0, i);

        if (i >= 60)
        {
            int minutes = i / 60;
            int seconds = i % 60;
            textField.text = $"{minutes}:{seconds:D2}";
        }
        else
        {
            textField.text = $"{i} sec";
        }
    }
}

