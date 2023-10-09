using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyMeter : MonoBehaviour
{
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }
    
    public void UpdateProgress(float currentProgress,float totalProgress)
    {
        image.fillAmount = currentProgress/totalProgress;
    }
}
