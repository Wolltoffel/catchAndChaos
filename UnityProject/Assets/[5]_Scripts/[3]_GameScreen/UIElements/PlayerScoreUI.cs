using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
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
        i = Math.Clamp(i, 0, 3);

        textField.text = i.ToString();
    }
}

