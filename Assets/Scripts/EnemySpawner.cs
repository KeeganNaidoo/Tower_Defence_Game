using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
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
    }
}
