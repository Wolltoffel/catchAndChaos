using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndCircles : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject converge;
    private Material material;
    private Coroutine coroutine;

    public Vector2 pos1;
    public Vector2 pos2;

    private void Awake()
    {
        if (image == null)
            return;
        material = image.GetComponent<Image>().material;
        material.SetFloat("_border", 0.04f);
        material.SetFloat("_Convergence", 0);
        material.SetFloat("_MinimumCircleSize", 0.16f);
        material.SetVector("_Position_1", new Vector2(0.25f, 0.25f));
        material.SetVector("_Position_2", new Vector2(0.75f, 0.75f));

        GetComponent<Canvas>().sortingOrder = 1;
    }

    public void ConvergeOn(Transform transform1, Transform transform2, float time = 2)
    {
        if (material == null)
            return;

        coroutine = StartCoroutine(_Converge(transform1, transform2, time));
    }

    private IEnumerator _Converge(Transform parentTransform, Transform childTransform, float time = 2)
    {
        float progress = 0;
        float initialTime = time;

        while (time > initialTime/2)
        {
            yield return null;

            float timeElapsed = initialTime - time;
            progress = 1 - Mathf.Cos(Mathf.PI * Mathf.Pow(timeElapsed, 1f) * 0.5f);

            Debug.Log($"Converge: {progress} - ElapsedTime: {timeElapsed}");

            pos1 = Camera.main.WorldToScreenPoint(parentTransform.position + Vector3.up * 1.2f);
            pos2 = Camera.main.WorldToScreenPoint(childTransform.position);

            material.SetVector("_Position_1", pos1);
            material.SetVector("_Position_2", pos2);
            material.SetFloat("_Convergence", progress);

            time -= Time.deltaTime;
        }

        material.SetFloat("_Convergence", 1);
        material.SetFloat("_MinimumCircleSize", 0.16f);

        while (time > 0)
        {
            yield return null;

            float timeElapsed = initialTime / 2 - time;

            progress = - Mathf.Pow(timeElapsed - 0.4f,2) + 0.314f;

            //Debug.Log($"Converge: {progress} - ElapsedTime: {timeElapsed}");

            pos1 = Camera.main.WorldToScreenPoint(parentTransform.position + Vector3.up * 1.2f);
            pos2 = Camera.main.WorldToScreenPoint(childTransform.position);

            material.SetVector("_Position_1", pos1);
            material.SetVector("_Position_2", pos2);
            material.SetFloat("_MinimumCircleSize", progress);

            time -= Time.deltaTime;
        }

        material.SetFloat("_MinimumCircleSize", 0);
        material.SetFloat("_border", 0f);

        coroutine = null;
    }
}
