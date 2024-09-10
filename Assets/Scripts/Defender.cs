using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public GameObject projectilePrefab;   // The projectile to shoot
    public Transform projectileSpawnPoint; // Where the projectile spawns from
    public float attackRange = 10f;       // Attack range of the defender
    public float attackCooldown = 2f;     // Time between attacks
    public float projectileSpeed = 20f;   // Speed of the projectile

    private float attackCooldownTimer;    // Timer to handle attack intervals

    void Update()
    {
        // Check for enemies within attack range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && attackCooldownTimer <= 0f)
            {
                ShootProjectile(enemy);
                attackCooldownTimer = attackCooldown; // Reset cooldown
                break; // Only shoot one enemy at a time
            }
        }

        // Decrease attack cooldown timer
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    // Function to shoot a projectile at the enemy
    void ShootProjectile(Enemy target)
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Set the direction and speed of the projectile
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(target.transform, projectileSpeed);
        }
    }

    // Optional: Draw attack range for easier debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
