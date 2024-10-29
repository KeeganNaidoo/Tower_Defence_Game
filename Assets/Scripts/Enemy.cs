using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float baseHealth = 100f;
    public float damage = 10f;
    public float attackRange = 2f;         // Distance at which enemy stops to attack
    public float attackCooldown = 2f;
    public float maxEnemyHealth;
    public float currentEnemyHealth;

    private Slider healthBarSlider;
    private GameObject healthBarInstance;
    public GameObject healthBarPrefab;
    private NavMeshAgent agent;
    private GameObject mainTower;
    private float attackCooldownTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainTower = GameObject.FindWithTag("MainTower");  // Ensure the tower is tagged properly

        if (mainTower != null)
        {
            agent.SetDestination(mainTower.transform.position);
            agent.stoppingDistance = attackRange - 0.5f;  // Ensure enemy stops slightly before attack range
        }

        InitializeHealth();  // Set initial health values
        InitializeHealthBar();  // Set up health bar UI
    }

    void Update()
    {
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
                agent.isStopped = false;  // Resume movement if outside attack range
            }
        }

        UpdateHealthBarPosition();

        // Destroy health bar if enemy dies
        if (currentEnemyHealth <= 0 && healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }

    void InitializeHealth()
    {
        // Set health values at the start
        maxEnemyHealth = baseHealth;
        currentEnemyHealth = maxEnemyHealth;
    }

    void InitializeHealthBar()
    {
        // Check if health bar prefab is assigned and create it
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("HealthBarPrefab is not assigned in the inspector for Enemy.");
        }

        // Set up the health bar slider
        healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
        }
        else
        {
            Debug.LogWarning("HealthBarSlider is not assigned for this enemy.");
        }

        // Attach the health bar to the canvas
        healthBarInstance.transform.SetParent(GameObject.Find("Canvas").transform);
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
            Destroy(gameObject);
            Debug.Log("Enemy died");
        }
    }

    public void InitializeEnemyStats(float difficultyMultiplier)
    {
        // Set health and speed based on difficulty multiplier
        maxEnemyHealth = baseHealth * difficultyMultiplier;
        currentEnemyHealth = maxEnemyHealth;

        // Ensure healthBarSlider is not null before setting values
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
        }
        else
        {
            Debug.LogWarning("HealthBarSlider is not assigned for this enemy.");
        }

        // Adjust speed based on difficulty multiplier
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
