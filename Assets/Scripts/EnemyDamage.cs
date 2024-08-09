using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;                  // dealt damage by enemy (can vary with each type of enemy)
    public PlayerHealth playerHealth;   // player health script variable
    public Player playerMovement;       // player movement script variable
    public EnemyHurt enemyHurt;

    // on collision, call player health's take damage function
    // + do player's knockback effect movement
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {           
            
            if (collision.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            else if (collision.transform.position.x > transform.position.x)
            {
                playerMovement.KnockFromRight = false;
            }
            
            if (!playerHealth.IsInvincible())
            {
                //Player knockback
                playerMovement.Knockback();
                playerHealth.TakeDamage(damage);
                if (playerMovement.has_counter) enemyHurt.TakeDamage(1);
            }
            
        }
    }
}
