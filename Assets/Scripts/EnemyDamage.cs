using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage;                  // dealt damage by enemy (can vary with each type of enemy)
    public PlayerHealth playerHealth;   // player health script variable
    public Player playerMovement;       // player movement script variable

    // on collision, call player health's take damage function
    // + do player's knockback effect movement
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(collision.gameObject.name);
           
            
            if (collision.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            else if (collision.transform.position.x > transform.position.x)
            {
                playerMovement.KnockFromRight = false;
            }
            playerMovement.Knockback();

            playerHealth.TakeDamage(damage);
        }
    }
}
