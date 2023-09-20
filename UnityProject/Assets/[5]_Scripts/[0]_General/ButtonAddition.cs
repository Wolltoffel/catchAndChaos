using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ButtonAddition : MonoBehaviour, IPointerEnterHandler,ISelectHandler, IDeselectHandler
{
    private Vector2 originalScaleButton;
    bool scaledUp;
    TextMeshProUGUI textMeshPro;
    float origninalSizeText;

    void Awake()
    {
        originalScaleButton = transform.localScale;
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro!=null)
            origninalSizeText = textMeshPro.fontSize;
    }

    public void OnSelect (BaseEventData baseEventData)
    {
        if (!scaledUp)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleUp(transform.localScale,originalScaleButton*1.5f));
        }
    }

    public void OnDeselect (BaseEventData baseEventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleDown(transform.localScale,originalScaleButton));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current!=null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public IEnumerator ScaleUp(Vector2 currentScale, Vector2 targetScale)
    {
        // Smoothly scales the Button
        Vector2 startScale = currentScale;;
        float startTime = Time.time;

        while (Vector2.Distance(currentScale, targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentScale = Vector2.Lerp(currentScale, targetScale, t);
            if (textMeshPro!=null)
                textMeshPro.fontSize = Mathf.Lerp(origninalSizeText,origninalSizeText*0.2f,t);
            transform.localScale = currentScale;
            yield return null;
        }

        scaledUp = true;
    }

    public IEnumerator ScaleDown(Vector2 currentScale, Vector2 targetScale)
    {
        // Smoothly scales the Button
        Vector2 startScale = currentScale;
        float currentFontSize = 0;
        if (textMeshPro!=null)
            currentFontSize = textMeshPro.fontSize;
        float startTime = Time.time;

        while (Vector2.Distance(currentScale, targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentScale = Vector2.Lerp(currentScale, targetScale, t);
            if (textMeshPro !=null)
                textMeshPro.fontSize = Mathf.Lerp(currentFontSize,origninalSizeText,t);
            transform.localScale = currentScale;
            yield return null;
        }

        scaledUp = false;
    }


}
