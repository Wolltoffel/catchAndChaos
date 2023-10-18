using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VentRollup : MonoBehaviour
{

    SkinnedMeshRenderer skinnedMeshRenderer;
    public bool isOpen;
    Coroutine coroutine;
    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer.GetBlendShapeWeight(0) >= 100)
            isOpen = true;
    }

    IEnumerator SetVentValue(float currentScale, float targetScale)
    {
        float startScale = currentScale;
        float startTime = Time.time;

        while (Mathf.Abs(currentScale - targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentScale = Mathf.Lerp(currentScale, targetScale, t);
            skinnedMeshRenderer.SetBlendShapeWeight(0, currentScale);
            yield return null;
        }
        coroutine = null;
        yield return null;
    }

    public void CloseVent()
    {
        if (isOpen)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(SetVentValue(skinnedMeshRenderer.GetBlendShapeWeight(0), 0));
        }
            
        isOpen = false;
    }

    public void OpenVent()
    {

        if (!isOpen)
        {
            isOpen = true;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(SetVentValue(skinnedMeshRenderer.GetBlendShapeWeight(0), 100));

            //SoundSystem.PlaySound("VentOpen3");
        }
            
        isOpen = true;
    }
}
