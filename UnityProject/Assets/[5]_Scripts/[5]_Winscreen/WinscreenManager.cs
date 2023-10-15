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
    [SerializeField] Transform parentAnchor, childAnchor, cameraAnchor;
    Characters winner;
    GameObject childInstance, parentInstance;

    //WinScreenStuff
    [SerializeField] private GameObject UIManagers;

    [Space]
    //CanvasSortingOrder
    [SerializeField] private Canvas canvasParent;
    [SerializeField] private Canvas canvasChild;
    [SerializeField] private Canvas canvasForeground;
    [SerializeField] private int sortingOrderParent;
    [SerializeField] private int sortingOrderChild;
    [SerializeField] private int sortingOrderForeground;

    private void Awake()
    {
        DetermineWinner();
    }

    private IEnumerator Start()
    {
        SpawnCharacters();
        //SetCameraPosition();
        SetUpUI();
        yield return null;
    }

    private void Update() => UpdateCanvasSortingOrder();

    private void SetUpUI()
    {
        UIManagers.SetActive(true);
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
        //Transform childAnchor = null;
        //Transform parentAnchor = null;

        //Spawn Winners at winner positon and loser at loser position
        //if (winner == Characters.Child)
        //{
        //    childAnchor = this.parentAnchor;
        //    parentAnchor = this.childAnchor;
        //}
        //else if (winner == Characters.Parent)
        //{
        //    parentAnchor = this.parentAnchor;
        //    childAnchor = this.childAnchor;
        //}

        CharacterInstantiator.InstantiateCharacter(Characters.Child, out childInstance,childAnchor,true);
        CharacterInstantiator.InstantiateCharacter (Characters.Parent, out parentInstance,parentAnchor,true);
        CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>().SetInteger("ChildIndex", 0);
        CharacterInstantiator.GetActiveCharacter(Characters.Parent).GetComponent<Animator>().SetInteger("MomIndex", 0);
    }

    void SetCameraPosition()
    {
        Camera.main.GetComponentInParent<CameraManager>().SetCameraAsMain();
        Camera.main.GetComponentInParent<CameraManager>().SetCameraPosition(cameraAnchor);
    }

    void UpdateCanvasSortingOrder()
    {
        canvasParent.sortingOrder = sortingOrderParent;
        canvasChild.sortingOrder = sortingOrderChild;
        canvasForeground.sortingOrder = sortingOrderForeground;
    }
}
