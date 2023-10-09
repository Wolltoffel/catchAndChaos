using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChaosMeterUI : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float currentValue, targetValue = 0;
    [SerializeField] private RectTransform maskTranfrom;
    private float maskMaximumHeight;

    private Coroutine lerpCoroutine;
    private float lerpFactor = 0.01f;

    private void Awake()
    {
        maskMaximumHeight = maskTranfrom.rect.height;
        SetValueOnMeter(0);
    }

    public void UpdateUI(float i)
    {
        targetValue = i;
        if (currentValue != targetValue)
        { 
            if (lerpCoroutine == null)
            {
                lerpCoroutine = StartCoroutine(LerpToTarget());
            }
        }
    }

    private IEnumerator LerpToTarget()
    {
        float offset = 0.01f;

        //Lerp while not at target position
        while (targetValue - offset >= currentValue || targetValue + offset <= currentValue)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, lerpFactor);
            SetValueOnMeter(currentValue);
            yield return null;
        }

        //Finalize lerp
        currentValue = targetValue;
        SetValueOnMeter(currentValue);

        lerpCoroutine = null;
    }

    private void SetValueOnMeter(float i)
    {
        //sets the height of the mask
        i = Mathf.Clamp01(i);
        Vector2 newSizeDelta = new(maskTranfrom.sizeDelta.x, maskMaximumHeight * i);
        maskTranfrom.sizeDelta = newSizeDelta;
    }
}

