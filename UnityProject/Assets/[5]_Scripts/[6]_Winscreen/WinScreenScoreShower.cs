using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenScoreShower : MonoBehaviour
{
   [SerializeField]TextMeshProUGUI textMeshProUGUI;

   void Start()
   {
        if (textMeshProUGUI==null)
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        SetText();
   }

   void SetText()
   {
        Characters winner = GameData.GetData<WinData>("WinData").winner;
        
        int childScore = GameData.GetData<PlayerData>("Child").tempScore;
        int parentScore = GameData.GetData<PlayerData>("Parent").tempScore;

        int winnerScore = 0;
        int loserScore = 0;

        if (winner== Characters.Child)
        {
            winnerScore = childScore;
            loserScore = parentScore;
        }
        else if (winner== Characters.Parent)
        {
            winnerScore = parentScore;
            loserScore = childScore;
        }

        textMeshProUGUI.text = winnerScore.ToString()+ " : "+ loserScore.ToString();   
   }
}
