using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenScoreShower : MonoBehaviour
{
   [SerializeField]TextMeshProUGUI scoreTextMeshPro;
   [SerializeField] TextMeshProUGUI namesTextMeshPro;

    void Start()
   {
        if (scoreTextMeshPro==null)
            scoreTextMeshPro = GetComponent<TextMeshProUGUI>();

        SetText();
   }

   void SetText()
   {
        WinData data = GameData.GetData<WinData>("WinData");
        Characters winner = data.winner;

        int winnerScore = 0;
        int loserScore = 0;

        if (winner== Characters.Child)
        {
            winnerScore = data.childScore;
            loserScore = data.parentScore;
        }
        else if (winner== Characters.Parent)
        {
            winnerScore = data.parentScore;
            loserScore = data.childScore;
        }

        namesTextMeshPro.text = "Mother" + " : " + "Child";
        scoreTextMeshPro.text = winnerScore.ToString()+ " : "+ loserScore.ToString();   
   }
}
