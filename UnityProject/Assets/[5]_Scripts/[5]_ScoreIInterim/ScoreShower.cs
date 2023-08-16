using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreShower : MonoBehaviour
{
   [SerializeField]TextMeshProUGUI scoresTMP;
    

    void Start()
    {
        if (scoresTMP== null)
            scoresTMP.GetComponent<TextMeshProUGUI>();
        ShowScores();
    }

    public void ShowScores()
    {
        int parentScore = GameData.GetData<PlayerData>("Parent").tempScore;
        int childScore = GameData.GetData<PlayerData>("Child").tempScore;

        string scores = childScore.ToString() + " : "+ parentScore.ToString();

        scoresTMP.text = scores;
    }

}
