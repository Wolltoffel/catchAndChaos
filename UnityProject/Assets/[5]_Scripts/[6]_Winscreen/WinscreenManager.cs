using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Could be removed
public class WinData: Object
{
    public readonly Characters winner;
    public WinData(Characters winner)
    {
        this.winner = winner;
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

        childData.ResetTempScores();
        parentData.ResetTempScores();

        //Could be removed
        WinData winData = new WinData (winner);
        GameData.SetData(winData, "WinData");
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
