using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenScoreShower : MonoBehaviour
{

   [SerializeField]TextMeshProUGUI scoreTextMeshProParent,scoreTextMeshProChild;

    void Start()
   {
        SetText();
   }

   void SetText()
   {
        WinData data = GameData.GetData<WinData>("WinData");
        Characters winner = data.winner;

        scoreTextMeshProParent.text = data.parentScore.ToString();
        scoreTextMeshProChild.text = data.childScore.ToString();
   }
}
