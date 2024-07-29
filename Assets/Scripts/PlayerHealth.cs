using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;       // default max health player starts with
    public int health;              // updated player health

    void Start()
    {
        health = maxHealth;
    }

    // damage subtracter from player's health depending on enemy
    // + player is destroyed if health reaches or is below 0
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
