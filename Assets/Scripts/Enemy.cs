using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float enemyHealth = 100f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float maxEnemyHealth = 100f;
    public float currentHealth;
    private Slider healthBarSlider;
    private GameObject healthBarInstance;

    public GameObject healthBarPrefab;  // The prefab with the health bar
    private NavMeshAgent agent;
    private GameObject mainTower;
    private float attackCooldownTimer;

    void Start()
    {
        
        
        agent = GetComponent<NavMeshAgent>();
        mainTower = GameObject.FindWithTag("MainTower"); // tag the main tower
        agent.SetDestination(mainTower.transform.position); // Enemy targets the main tower
        
        currentHealth = maxEnemyHealth;
        // Instantiate the health bar and assign it
        healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        // Set the health bar's slider
        healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxEnemyHealth;
            healthBarSlider.value = currentHealth;
        }

        // Parent the health bar to the Canvas 
        healthBarInstance.transform.SetParent(GameObject.Find("WorldCanvas").transform);
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
        // Update the health bar's position to follow the enemy
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + new Vector3(0, 2, 0);
        }

        // Destroy the health bar when the enemy dies
        if (currentHealth <= 0 && healthBarInstance != null)
        {
            Destroy(healthBarInstance);
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
        enemyHealth -= damage;

        // Update the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }

        if (enemyHealth <= 0f)
        {
            Destroy(gameObject);
            Debug.Log("Enemy died");
        }
    }

    
}
