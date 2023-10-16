using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LoadingScreenManager : MonoBehaviour
{
    public float loadTime = 2f;

    [Header("Local References")]
    private Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        loadTime = animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, loadTime);
    }
}
