using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public float towerHealth = 500f;        // Tower health
    public float maxTowerHealth = 500f;
    public float attackDamage = 20f;        // Damage dealt to enemies
    public float attackRange = 10f;         // Range to detect and attack enemies
    public float attackCooldown = 2f;       // Time between each attack
    public Slider towerHealthBar;
    public Text towerHealthText;

    private float attackCooldownTimer;      // Timer to manage attack intervals
    
    void Start()
    {
        // Set initial health bar value
        if (towerHealthBar != null)
        {
            towerHealthBar.maxValue = maxTowerHealth;
            towerHealthBar.value = towerHealth;
        }

        // Set initial health text
        if (towerHealthText != null)
        {
            towerHealthText.text = $"Health: {towerHealth}/{maxTowerHealth}";
        }
    }

    void Update()
    {
        // Check for enemies within range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            // If an enemy is found within range
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && attackCooldownTimer <= 0f)
            {
                AttackEnemy(enemy);
                attackCooldownTimer = attackCooldown; // Reset cooldown after attacking
                break; // Only attack one enemy at a time
            }
        }

        // Decrease attack cooldown timer
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        
        // Update the health bar and text if the health changes
        if (towerHealthBar != null)
        {
            towerHealthBar.value = towerHealth;
        }

        if (towerHealthText != null)
        {
            towerHealthText.text = $"Health: {towerHealth}/{maxTowerHealth}";
        }
    }

    // Tower attacks the enemy
    void AttackEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(attackDamage);
            
        }
    }

    // Tower takes damage from enemies
    public void TakeDamage(float damage)
    {
        towerHealth -= damage;

        Debug.Log("Tower health: " + towerHealth);  

        if (towerHealth <= 0f)
        {
            DestroyTower();
        }
    }

    // Destroy the tower when health is depleted
    void DestroyTower()
    {
        // Tower destroyed, trigger game over or any event
        Debug.Log("Tower has been destroyed!");
        // Implement game over logic here
        Destroy(gameObject);
    }

    // Optional: Draw attack range for easier debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
