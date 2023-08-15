using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardButtonPrompts : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField]float initialDistance = 1;
    float intendedSize;

    void Start()
    {
        mainCamera = Camera.main;
        intendedSize = transform.localScale.x;
    }

    void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;
        AdjustSize();
    }

    void AdjustSize()
    {
        float distance = Vector3.Distance (transform.position,mainCamera.transform.position);
        float adjustedScale = intendedSize* (initialDistance/distance);
        transform.localScale = new Vector3(adjustedScale, adjustedScale,adjustedScale);
    }
}
