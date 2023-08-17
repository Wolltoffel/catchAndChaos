using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAddition : MonoBehaviour, IPointerEnterHandler,ISelectHandler, IDeselectHandler
{
    private Vector2 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect (BaseEventData baseEventData)
    {
        StopAllCoroutines();
        StartCoroutine(Resize(originalScale,transform.localScale*1.5f));
    }

    public void OnDeselect (BaseEventData baseEventData)
    {

        StopAllCoroutines();
        StartCoroutine(Resize(transform.localScale,originalScale));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    IEnumerator Resize(Vector2 currentScale, Vector2 targetScale)
    {
        // Smoothly scales the Button
        Vector2 startScale = currentScale;;
        float startTime = Time.time;

        while (Vector2.Distance(currentScale, targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentScale = Vector2.Lerp(currentScale, targetScale, t);
            transform.localScale = currentScale;
            yield return null;
        }
    }

}
