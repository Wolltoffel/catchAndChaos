using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TextEditHelper
{
    public static class TextEditHelper
    {
        public static string AddText(TextMeshProUGUI tmp, string text)
        {
            string currentText = tmp.text;
            currentText += " " + text;
            tmp.text = currentText;
            return currentText;
        }

        public static string SetText(TextMeshProUGUI tmp, string text)
        {
            tmp.text = text;
            return text;
        }
    }
}