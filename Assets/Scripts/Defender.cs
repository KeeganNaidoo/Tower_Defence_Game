using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public GameObject projectilePrefab;      // The projectile to shoot
    public Transform projectileSpawnPoint;   // Where the projectile spawns from
    public float attackRange = 10f;          // Attack range of the defender
    public float attackCooldown = 2f;        // Time between attacks
    public float projectileSpeed = 20f;      // Speed of the projectile
    public float rotationSpeed = 5f;         // Speed at which the defender rotates towards the enemy

    private float attackCooldownTimer;       // Timer to handle attack intervals
    private Transform currentTarget;         // The enemy the defender is currently targeting

    void Update()
    {
        // Look for enemies within range
        FindTarget();

        // If we have a target, rotate and shoot
        if (currentTarget != null)
        {
            // Rotate towards the enemy
            RotateTowards(currentTarget);

            // Shoot if the cooldown timer is ready
            if (attackCooldownTimer <= 0f)
            {
                ShootProjectile(currentTarget);
                attackCooldownTimer = attackCooldown;  // Reset cooldown
            }

            // Decrease attack cooldown timer
            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
        }
    }

    // Function to find a target enemy within range
    void FindTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                currentTarget = enemy.transform; // Set the first enemy found as the target
                break; // Focus on the first enemy found
            }
        }
    }

    // Function to shoot a projectile at the current target
    void ShootProjectile(Transform target)
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Set the direction and speed of the projectile
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(target, projectileSpeed);  // Pass the target's transform to the projectile
        }
    }

    // Rotate the defender to face the target enemy
    void RotateTowards(Transform target)
    {
        // Find the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Calculate the target rotation
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Rotate on the Y axis only

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Optional: Draw attack range for easier debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
