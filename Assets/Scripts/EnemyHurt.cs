using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurt : MonoBehaviour
{
    const int WEAPON_LAYER = 8;
    [SerializeField] int MaxHealth = 2;

    int health;

    private void Awake()
    {
        health = MaxHealth;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer == WEAPON_LAYER)
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        health--;
        Debug.Log("Enemy remaining health: " +  health);
        if (health <= 0)
        {
            Destroy(this.transform.parent.gameObject);
        }
    }
}
