using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreTemplateManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parentMask;
    [SerializeField]
    private GameObject childMask;
    [SerializeField]
    private RectTransform rootTransform;
    [SerializeField]
    private TextMeshProUGUI nameTextMesh;
    [SerializeField]
    private TextMeshProUGUI scoreTextMesh;
    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        animator.enabled = true;
    }

    public void SetCharacter(Characters character)
    {
        if (character == Characters.Child)
        {
            parentMask.SetActive(false);
        }
        else
        {
            childMask.SetActive(false);
        }
    }

    public void SetName(string name)
    {
        nameTextMesh.text = name;
    }

    public void SetScore(int score)
    {
        scoreTextMesh.text = $"{score}";
    }

    public void RaiseScore(int raisedScore)
    {
        scoreTextMesh.text = $"{raisedScore}";
        SoundSystem.PlaySound("pointsUp");
        animator.SetTrigger("Win");
    }

    public Vector2 SetPosition(Vector2 position)
    {
        rootTransform.position = position;
        return position;
    }

    public void SetParent(Transform transform)
    {
        gameObject.transform.parent = transform;
    }
}
