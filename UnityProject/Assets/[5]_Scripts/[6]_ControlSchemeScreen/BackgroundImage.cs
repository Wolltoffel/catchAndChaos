using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] GameObject opaqueBK,transparentBK;

    void Awake()
    {
        
    }
    public void SwitchToTransparentBK()
    {
        opaqueBK.SetActive(false);
        transparentBK.SetActive(true);
    }
}
