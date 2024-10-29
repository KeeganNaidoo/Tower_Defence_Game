using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float enemyHealth = 100f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float maxEnemyHealth = 100f;
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
        mainTower = GameObject.FindWithTag("MainTower"); // tag the main tower
        if (mainTower != null) agent.SetDestination(mainTower.transform.position);

        currentEnemyHealth = maxEnemyHealth;
        enemyHealth = maxEnemyHealth;  // Set initial health values

        // Instantiate the health bar
        healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        // Set the health bar's slider
        healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentEnemyHealth;
        }

        // Parent the health bar to the Canvas 
        healthBarInstance.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    void Update()
    {
        if (mainTower != null)
        {
            float distance = Vector3.Distance(transform.position, mainTower.transform.position);

            if (distance <= attackRange)
            {
                AttackTower();
            }
        }

        // Update health bar position to follow enemy
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + new Vector3(0, 2, 0);
        }

        // Destroy the health bar when the enemy dies
        if (currentEnemyHealth <= 0 && healthBarInstance != null)
        {
            Destroy(healthBarInstance);
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
        enemyHealth -= amount;
        currentEnemyHealth = enemyHealth;  // Sync current health

        // Update the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentEnemyHealth;
        }

        if (enemyHealth <= 0f)
        {
            Destroy(gameObject);
            Debug.Log("Enemy died");
        }
    }
}
