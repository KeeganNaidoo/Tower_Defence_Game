using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float baseHealth = 100f;
    public float damage = 10f;
    public float attackRange = 2f;        // Range at which enemy can attack the tower
    public float attackCooldown = 2f;
    public float maxEnemyHealth;
    public float currentEnemyHealth;

    private Slider healthBarSlider;
    private GameObject healthBarInstance;
    public GameObject healthBarPrefab;
    private NavMeshAgent agent;
    private GameObject mainTower;
    public GameObject coinPrefab;         // Reference to the coin prefab
    private float attackCooldownTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainTower = GameObject.FindWithTag("MainTower");

        if (mainTower != null)
        {
            agent.SetDestination(mainTower.transform.position); // Enemy heads towards the main tower
        }

        InitializeHealth();
        InitializeHealthBar();
    }

    void Update()
    {
        if (currentEnemyHealth <= 0) return;

        if (mainTower != null)
        {
            float distanceToTower = Vector3.Distance(transform.position, mainTower.transform.position);

            // Check if enemy is within attack range and attack the tower
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

        healthBarSlider = healthBarInstance?.GetComponentInChildren<Slider>();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
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
        DropCoin();
        Destroy(healthBarInstance);
        Destroy(gameObject);
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

        if (agent != null)
        {
            agent.speed *= difficultyMultiplier;
        }
    }
}

