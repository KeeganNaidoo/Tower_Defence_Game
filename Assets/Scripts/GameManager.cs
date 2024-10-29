using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;       // Singleton instance
    private EnemySpawner enemySpawner;
    private int mainTowerHealth = 100;
    private float averageKillTime = 0;
    private int totalKills = 0;
    private int playerCoins = 0;

    // References to UI elements
    public Text coinBalanceText;              // UI text for coin balance
    public Text mainTowerHealthText;          // UI text for main tower health

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        UpdateMainTowerHealthUI();             // Display initial health
        UpdateCoinUI();                        // Display initial coin balance
    }

    // Method to handle adding coins to the player balance
    public void AddCoins(int amount)
    {
        playerCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinBalanceText != null)
        {
            coinBalanceText.text = "Coins: " + playerCoins.ToString();
        }
    }

    // Method to handle updates to main tower health
    public void UpdateMainTowerHealth(int damage)
    {
        mainTowerHealth -= damage;
        UpdateMainTowerHealthUI();

        if (mainTowerHealth <= 0)
        {
            Debug.Log("Game Over! Main Tower Destroyed.");
            GameOver();
        }
    }

    private void UpdateMainTowerHealthUI()
    {
        if (mainTowerHealthText != null)
        {
            mainTowerHealthText.text = "Tower Health: " + mainTowerHealth.ToString();
        }
    }

    // Method to manage difficulty based on kill time
    public void UpdateKillStats(float killTime)
    {
        // Calculate average kill time for difficulty adjustment
        averageKillTime = ((averageKillTime * totalKills) + killTime) / (totalKills + 1);
        totalKills++;

        // Adjust difficulty based on average kill time
        if (averageKillTime < 1.0f)
        {
            enemySpawner.IncreaseDifficulty();
        }
        else if (averageKillTime > 2.0f)
        {
            enemySpawner.DecreaseDifficulty();
        }
    }

    private void GameOver()
    {
        // Actions to perform on game over (e.g., show game-over screen)
        // Additional game-over logic can be added here
        Debug.Log("Game Over - Game Over screen can be triggered here.");
    }
}

