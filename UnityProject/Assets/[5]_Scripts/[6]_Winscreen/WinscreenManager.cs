using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinData: Object
{
    public readonly Characters winner;
    public readonly int parentScore;
    public readonly int childScore;

    public WinData(Characters winner, int parentScore, int childScore)
    {
        this.winner = winner;
        this.parentScore = parentScore;
        this.childScore = childScore;
    }
}


public class WinscreenManager : MonoBehaviour
{
    [SerializeField] Transform winnerAnchor, loserAnchor, cameraAnchor;
    Characters winner;
    GameObject childInstance, parentInstance;

    void Awake()
    {
        DetermineWinner();
        SpawnCharacters();
        SetCameraPosition();
        //SetUpUI();
    }

    private void SetUpUI()
    {
        Debug.Log("Not set up.");
    }

    void DetermineWinner()
    {
        var childData = GameData.GetData<PlayerData>("Child");
        var parentData = GameData.GetData<PlayerData>("Parent");

        if (childData.tempScore > parentData.tempScore)
            winner = Characters.Child;
        else if (childData.tempScore == parentData.tempScore)
            winner = Characters.Child;
        else 
            winner = Characters.Parent;

        WinData winData = new WinData (winner, parentData.tempScore, childData.tempScore);
        GameData.SetData(winData, "WinData");

        childData.ResetTempScores();
        parentData.ResetTempScores();
    }

    void SpawnCharacters()
    {
        Transform childAnchor = null;
        Transform parentAnchor = null;

        //Spawn Winners at winner positon and loser at loser position
        if (winner == Characters.Child)
        {
            childAnchor = winnerAnchor;
            parentAnchor = loserAnchor;
        }
        else if (winner == Characters.Parent)
        {
            parentAnchor = winnerAnchor;
            childAnchor = loserAnchor;
        }

        CharacterInstantiator.InstantiateCharacter(Characters.Child, out childInstance,childAnchor);
        CharacterInstantiator.InstantiateCharacter (Characters.Parent, out parentInstance,parentAnchor);
    }

    void SetCameraPosition()
    {
        Camera.main.transform.position = cameraAnchor.position;
        Camera.main.transform.rotation = cameraAnchor.rotation;
    }
}
