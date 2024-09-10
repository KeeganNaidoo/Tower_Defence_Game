using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private GameObject mainTower;
    private float attackCooldownTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainTower = GameObject.FindWithTag("MainTower"); // tag the main tower
        agent.SetDestination(mainTower.transform.position); // Enemy targets the main tower
    }

    void Update()
    {
        if (mainTower != null)
        {
            // Check distance to the main tower
            float distance = Vector3.Distance(transform.position, mainTower.transform.position);

            if (distance <= attackRange)
            {
                AttackTower();
            }
        }
    }

    void AttackTower()
    {
        if (attackCooldownTimer <= 0f)
        {
            // Find the tower script and deal damage to it
            Tower tower = mainTower.GetComponent<Tower>();
            if (tower != null)
            {
                tower.TakeDamage(damage);
            }

            // Reset cooldown timer
            attackCooldownTimer = attackCooldown;
        }
        else
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); // Destroy the enemy when health reaches 0
    }
}
