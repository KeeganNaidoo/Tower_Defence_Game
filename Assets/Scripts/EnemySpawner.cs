using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemyHealthBarPrefab;  // Reference the health bar prefab
    public Transform[] spawnLocations;
    public float spawnInterval = 5f;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval; // Reset timer
        }
    }

    void SpawnEnemy()
    {
        // Choose a random spawn location
        Transform randomSpawn = spawnLocations[Random.Range(0, spawnLocations.Length)];
        Instantiate(enemyPrefab, randomSpawn.position, Quaternion.identity);
        
            // Assign the health bar prefab to the enemy
        Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();
        enemyScript.healthBarPrefab = enemyHealthBarPrefab;
    }
}
