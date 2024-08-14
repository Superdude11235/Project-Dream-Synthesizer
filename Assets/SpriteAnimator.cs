using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    private Animator animator;

    private const string IS_GROUNDED = "IsGrounded";
    private const string MOVE_SPEED = "MoveSpeed";
    private const string JUMP_SPEED = "JumpSpeed";
    private const string IS_SLIDING = "IsSliding";
    private const string IS_HURT = "IsHurt";
    private const string IS_CROUCHING = "IsCrouching";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetGrounded(bool is_grounded)
    {
        animator.SetBool(IS_GROUNDED, is_grounded);
    }

    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat(MOVE_SPEED, speed);
    }

    public void SetJumpSpeed (float speed)
    {
        animator.SetFloat(JUMP_SPEED, speed);
    }

    public void SetCrouching(bool is_crouching)
    {
        animator.SetBool(IS_CROUCHING, is_crouching);
    }

    public void SetSliding(bool is_sliding)
    {
        animator.SetBool(IS_SLIDING, is_sliding);
    }

    public void SetHurt(bool is_hurt)
    {
        animator.SetBool(IS_HURT, is_hurt);
    }
}
