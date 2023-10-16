using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UIAnimationLengthManager : MonoBehaviour
{
    public float animationLength = 2f;

    [Header("Local References")]
    private Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animationLength);
    }
}
