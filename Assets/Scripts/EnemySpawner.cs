using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Different enemy types to spawn
    public Transform[] spawnLocations; // Predefined spawn points for enemies
    public int initialEnemyCount = 5; // Base number of enemies to spawn in a wave
    public float spawnInterval = 2.0f; // Interval between enemy spawns within a wave
    public float waveInterval = 5.0f; // Interval between waves
    private int currentWave = 1;
    private float difficultyMultiplier = 1.0f;

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (true)
        {
            // Delay before starting the next wave
            yield return new WaitForSeconds(waveInterval);

            // Calculate the number of enemies for the current wave
            int enemiesToSpawn = Mathf.RoundToInt(initialEnemyCount * difficultyMultiplier);
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            currentWave++;
            AdjustDifficulty();
        }
    }

    private void SpawnEnemy()
    {
        // Check if enemyPrefabs has at least one element
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned in EnemySpawner.");
            return; // Exit the method if there are no prefabs
        }

        GameObject enemyPrefab;

        // Select enemy type based on current wave progression
        if (currentWave < 3)
        {
            enemyPrefab = enemyPrefabs[0]; // Basic enemy for early waves
        }
        else if (currentWave < 5)
        {
            // Ensure there are enough elements to prevent index errors
            if (enemyPrefabs.Count >= 2)
            {
                enemyPrefab = enemyPrefabs[Random.Range(0, 2)]; // Mix of basic and medium enemies
            }
            else
            {
                enemyPrefab = enemyPrefabs[0]; // Fallback in case there aren't enough prefabs
            }
        }
        else
        {
            enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]; // Include all types for later waves
        }

        // Select a random spawn point and spawn the enemy there
        Transform spawnPoint = GetSpawnLocation();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No valid spawn point found.");
            return; // Exit if no spawn point is available
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().InitializeEnemyStats(difficultyMultiplier);
    }


    private void AdjustDifficulty()
    {
        // Increase difficulty multiplier with each wave
        difficultyMultiplier += 0.1f * currentWave;
        spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.1f); // Increase spawn rate gradually
    }

    private Transform GetSpawnLocation()
    {
        // Randomly selects a spawn point from predefined locations
        return spawnLocations[Random.Range(0, spawnLocations.Length)];
    }

    // Method to increase difficulty externally (e.g., from GameManager)
    public void IncreaseDifficulty()
    {
        difficultyMultiplier += 0.2f;
        spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.05f); // Optionally decrease spawn interval for higher difficulty
        Debug.Log("Difficulty increased!");
    }

    // Method to decrease difficulty externally (e.g., from GameManager)
    public void DecreaseDifficulty()
    {
        difficultyMultiplier = Mathf.Max(1.0f, difficultyMultiplier - 0.2f);
        spawnInterval = Mathf.Min(2.0f, spawnInterval + 0.05f); // Optionally increase spawn interval for lower difficulty
        Debug.Log("Difficulty decreased!");
    }
}
