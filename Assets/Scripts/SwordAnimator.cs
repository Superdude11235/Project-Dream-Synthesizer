using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimator : MonoBehaviour
{
    private Animator animator;

    private const string SWORD_SWUNG = "SwordSwung";

    private const string SWORD_IDLE = "SwordIdleTest";

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        
    }

    public void SwingSword()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(SWORD_IDLE))
        {
            animator.SetTrigger(SWORD_SWUNG);
        }
       
    }
}
