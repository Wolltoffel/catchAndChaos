using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAnimationManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string childAnimationWin;
    [SerializeField] string parentAnimationWin;

    WinData windata;

    void Start()
    {
        windata = GameData.GetData<WinData>("WinData");
        PickAndPlayUIAnim();
        PickAndPlayCharacterAnim();
    }

    void PickAndPlayUIAnim()
    {   
        if (windata.winner == Characters.Child )
            animator.Play(childAnimationWin,0);
        else
            animator.Play(parentAnimationWin,0);
    }

    void PickAndPlayCharacterAnim()
    {   
        
        Animator childAnimator = CharacterInstantiator.GetActiveCharacter(Characters.Child).GetComponent<Animator>();
        Animator parentAnimator = CharacterInstantiator.GetActiveCharacter(Characters.Parent).GetComponent<Animator>();

        if (windata.winner == Characters.Child)
        {
            childAnimator.SetInteger("ChildIndex",1);
            parentAnimator.SetInteger("MomIndex",8);
        }
        else
        {
            childAnimator.SetInteger("ChildIndex",8);
            parentAnimator.SetInteger("MomIndex",9);
        }
    }
}
