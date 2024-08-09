using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int baseMaxHealth = 3;
    public int maxHealth = 3;       // default max health player starts with
    public int health;              // updated player health

    private int enemies_killed = 0;

    [SerializeField] private float InvincibilityTime = 1.5f;
    private float iframeTimer = 0;

    //Player reference
    private Player player;

    //HP Drain values
    [SerializeField] private int min_enemies_drain = 5;
    [SerializeField] private int drain_value = 1;

    //Armor values
    [SerializeField] private int leather_armor_health = 1;
    [SerializeField] private int iron_armor_health = 2;


    void Awake()
    {
        health = maxHealth;
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        Events.Enemydied += TrackEnemiesKilled;
    }

    private void OnDisable()
    {
        Events.Enemydied -= TrackEnemiesKilled;
    }

    private void FixedUpdate()
    {
        HandleTimers();
    }

    private void HandleTimers()
    {
        //Iframe timer
        if (iframeTimer > 0) iframeTimer -= Time.deltaTime;
    }

    public bool IsInvincible()
    {
        return (iframeTimer > 0) || player.IsSliding();
    }

    // damage subtracter from player's health depending on enemy
    // + player is destroyed if health reaches or is below 0
    public void TakeDamage(int damage)
    {
        if (IsInvincible()) return;
        iframeTimer = InvincibilityTime;
        health -= damage;
        if (health <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        //Temporary; freezes scene
        Debug.Log("Game over!");
        Time.timeScale = 0;
    }

    private void TrackEnemiesKilled()
    {
        //Only call this function if the player has HP Drain; otherwise, reset enemies killed count
        if (!player.HasHPDrain())
        {
            enemies_killed = 0;
        }
        else
        {
            enemies_killed++;
            if (enemies_killed >= min_enemies_drain)
            {
                enemies_killed -= min_enemies_drain;
                health += drain_value;
                health = Mathf.Min(health, maxHealth);
                print("Health drained: " + drain_value);
            }
        }

    }

    public void HandleArmorHealth(string armorType, bool has_hp_up)
    {
        if (armorType == "Leather") maxHealth = baseMaxHealth + leather_armor_health;
        else if (armorType == "Iron") maxHealth = baseMaxHealth + iron_armor_health;
        else maxHealth = baseMaxHealth;
        if (has_hp_up) maxHealth++;
        health = Mathf.Min(health, maxHealth);


    }


    
}
