using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text enemiesAliveText;
    private int enemiesAlive = 0;

    void Start()
    {
        UpdateEnemiesAliveText();
    }

    public void EnemySpawned()
    {
        enemiesAlive++;
        UpdateEnemiesAliveText();
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
        UpdateEnemiesAliveText();
    }

    void UpdateEnemiesAliveText()
    {
        if (enemiesAliveText != null)
        {
            enemiesAliveText.text = $"Enemies Alive: {enemiesAlive}";
        }
    }
}
