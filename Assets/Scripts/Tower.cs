using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public float towerHealth = 500f;
    public float maxTowerHealth = 500f;
    public float attackDamage = 20f;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;
    public Slider towerHealthBar;             // Health slider UI component
    public Text towerHealthText;              // Health text UI component

    private float attackCooldownTimer;

    void Start()
    {
        // Initialize the health bar and text
        UpdateHealthUI();
    }

    void Update()
    {
        // Check for enemies within range and attack if cooldown allows
        if (attackCooldownTimer <= 0f)
        {
            AttackNearestEnemy();
            attackCooldownTimer = attackCooldown;
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    // Updates the health bar and health text UI to reflect current health
    void UpdateHealthUI()
    {
        if (towerHealthBar != null)
        {
            towerHealthBar.maxValue = maxTowerHealth;
            towerHealthBar.value = towerHealth;
        }

        if (towerHealthText != null)
        {
            towerHealthText.text = $"Health: {towerHealth}/{maxTowerHealth}";
        }
    }

    // Finds and attacks the nearest enemy within range
    void AttackNearestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);
        Enemy nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null)
        {
            FireProjectile(nearestEnemy);
        }
    }

    // Fires a projectile toward the specified enemy
    void FireProjectile(Enemy targetEnemy)
    {
        if (projectilePrefab != null && targetEnemy != null)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileScript = projectileInstance.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.Initialize(targetEnemy.transform, attackDamage);
            }
        }
    }

    // Reduces tower health and updates UI upon taking damage
    public void TakeDamage(float damage)
    {
        towerHealth -= damage;
        towerHealth = Mathf.Clamp(towerHealth, 0, maxTowerHealth);  // Ensure health doesn't go below 0

        Debug.Log("Tower health: " + towerHealth);

        UpdateHealthUI();  // Update UI immediately after taking damage

        if (towerHealth <= 0f)
        {
            DestroyTower();
        }
    }

    // Destroy the tower when health is depleted
    void DestroyTower()
    {
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

