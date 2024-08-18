using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YalmonAnimator : MonoBehaviour
{
    public EnemyMovement enemyMovement;
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        animator.SetBool("IsAttacking", enemyMovement.isChasing);
    }
}
