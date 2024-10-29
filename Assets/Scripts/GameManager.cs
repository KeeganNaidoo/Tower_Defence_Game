using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private int mainTowerHealth = 100;
    private float averageKillTime = 0;
    private int totalKills = 0;

    private void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    public void UpdateKillStats(float killTime)
    {
        // Calculate the average time it takes for defenders to eliminate enemies
        averageKillTime = ((averageKillTime * totalKills) + killTime) / (totalKills + 1);
        totalKills++;

        // Adjust difficulty based on the average kill time of defenders
        if (averageKillTime < 1.0f)
        {
            enemySpawner.IncreaseDifficulty();
        }
        else if (averageKillTime > 2.0f)
        {
            enemySpawner.DecreaseDifficulty();
        }
    }

    public void UpdateMainTowerHealth(int damage)
    {
        // Reduce main tower health when enemies reach it
        mainTowerHealth -= damage;
        if (mainTowerHealth <= 0)
        {
            // Trigger game-over state if main tower is destroyed
            Debug.Log("Game Over! Main Tower Destroyed.");
        }
    }
}
