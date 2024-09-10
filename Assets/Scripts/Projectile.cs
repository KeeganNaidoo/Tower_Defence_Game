using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    public float damage = 20f;

    public void Initialize(Transform enemyTarget, float projectileSpeed)
    {
        target = enemyTarget;
        speed = projectileSpeed;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy the projectile if there is no target
            return;
        }

        // Move the projectile towards the enemy
        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            // Hit the target
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        // Apply damage to the enemy
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Destroy the projectile after hitting the target
        Destroy(gameObject);
    }
}
