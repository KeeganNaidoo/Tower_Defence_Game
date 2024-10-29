using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float baseHealth = 100f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float maxEnemyHealth;
    public float currentEnemyHealth;

    private Slider healthBarSlider;
    private GameObject healthBarInstance;
    public GameObject healthBarPrefab;
    private NavMeshAgent agent;
    private GameObject mainTower;
    public GameObject coinPrefab;  // Reference to the coin prefab
    private float attackCooldownTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainTower = GameObject.FindWithTag("MainTower");

        if (mainTower != null)
        {
            agent.SetDestination(mainTower.transform.position);
            agent.stoppingDistance = attackRange - 0.5f;
        }

        InitializeHealth();
        InitializeHealthBar();
    }

    void Update()
    {
        if (currentEnemyHealth <= 0) return; // Exit update if enemy is dead

        if (mainTower != null)
        {
            float distanceToTower = Vector3.Distance(transform.position, mainTower.transform.position);

            // Stop moving if within attack range and start attacking
            if (distanceToTower <= attackRange)
            {
                agent.isStopped = true;
                AttackTower();
            }
            else
            {
                agent.isStopped = false;
            }
        }

        UpdateHealthBarPosition();
    }

    void InitializeHealth()
    {
        maxEnemyHealth = baseHealth;
        currentEnemyHealth = maxEnemyHealth;
    }

    void InitializeHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            healthBarInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        }
        else
        {
            Debug.LogWarning("HealthBarPrefab is not assigned in the inspector for Enemy.");
        }

        healthBarSlider = healthBarInstance?.GetComponentInChildren<Slider>();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
        }
        else
        {
            Debug.LogWarning("HealthBarSlider is not assigned for this enemy.");
        }
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + new Vector3(0, 2, 0);
        }
    }

    void AttackTower()
    {
        if (attackCooldownTimer <= 0f)
        {
            Tower tower = mainTower.GetComponent<Tower>();
            if (tower != null)
            {
                tower.TakeDamage(damage);
            }
            attackCooldownTimer = attackCooldown;
        }
        else
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        currentEnemyHealth -= amount;

        // Update the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentEnemyHealth;
        }

        if (currentEnemyHealth <= 0f)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        DropCoin();  // Drop a coin on death
        Destroy(healthBarInstance);  // Destroy the health bar
        Destroy(gameObject);  // Destroy the enemy object
        Debug.Log("Enemy died");
    }

    void DropCoin()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void InitializeEnemyStats(float difficultyMultiplier)
    {
        maxEnemyHealth = baseHealth * difficultyMultiplier;
        currentEnemyHealth = maxEnemyHealth;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
        }
        else
        {
            Debug.LogWarning("HealthBarSlider is not assigned for this enemy.");
        }

        if (agent != null)
        {
            agent.speed *= difficultyMultiplier;
        }
        else
        {
            Debug.LogWarning("NavMeshAgent component is missing on this enemy.");
        }
    }
}

