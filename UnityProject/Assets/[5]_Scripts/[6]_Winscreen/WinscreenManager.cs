using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    
    void DetermineWinner()
    {
        if (GameData.GetData<PlayerData>("Child").tempScore>=3)
            winner = Characters.Child;
        else if (GameData.GetData<PlayerData>("Parent").tempScore>=3)
            winner = Characters.Parent;

        WinData winData = new WinData (winner);
        GameData.SetData(winData, "WinData");
    }

    void SpawnCharacters()
    {
        Transform childAnchor = null;
        Transform parentAnchor = null;

        Characters winner = GameData.GetData<WinData>("WinData").winner;

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
