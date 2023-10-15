using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTemplateManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform rootTransform;
    [SerializeField]
    private TextMeshProUGUI nameTextMesh;
    [SerializeField]
    private TextMeshProUGUI scoreTextMesh;

    public void SetName(string name)
    {
        nameTextMesh.text = name;
    }

    public void SetScore(int score)
    {
        scoreTextMesh.text = $"Score: {score}";
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
