using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VentRollup : MonoBehaviour
{

    SkinnedMeshRenderer skinnedMeshRenderer;
    bool isOpen;
    Coroutine coroutine;
    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void SetUpObjects() 
    {
        float ventValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
    }

    IEnumerator SetVentValue(float currentScale, float targetScale)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        float startScale = currentScale;
        float startTime = Time.time;

        while (Mathf.Abs(currentScale - targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentScale = Mathf.Lerp(currentScale, targetScale, t);
            skinnedMeshRenderer.SetBlendShapeWeight(0, currentScale);
            yield return null;
        }
    }

    public void Toogle() 
    { 
        if (isOpen)
        {
            coroutine = StartCoroutine(SetVentValue(skinnedMeshRenderer.GetBlendShapeWeight(0), 100));
        }
        else
        {
            coroutine = StartCoroutine(SetVentValue(skinnedMeshRenderer.GetBlendShapeWeight(0), 0));
        }

        isOpen = !isOpen;
    }
}
