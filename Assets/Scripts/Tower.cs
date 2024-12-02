using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public float towerHealth = 500f;
    public float maxTowerHealth = 500f;
    public float attackDamage = 20f;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    public Slider towerHealthBar;
    public Text towerHealthText;

    private float attackCooldownTimer;

    void Start()
    {
        if (towerHealthBar != null)
        {
            towerHealthBar.maxValue = maxTowerHealth;
            towerHealthBar.value = towerHealth;
        }

        if (towerHealthText != null)
        {
            towerHealthText.text = $"Health: {towerHealth}/{maxTowerHealth}";
        }
    }

    void Update()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && attackCooldownTimer <= 0f)
            {
                AttackEnemy(enemy);
                attackCooldownTimer = attackCooldown;
                break;
            }
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (towerHealthBar != null)
        {
            towerHealthBar.value = towerHealth;
        }

        if (towerHealthText != null)
        {
            towerHealthText.text = $"Health: {towerHealth}/{maxTowerHealth}";
        }
    }

    void AttackEnemy(Enemy enemy)
    {
        enemy.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        towerHealth -= damage;
        if (towerHealth <= 0f)
        {
            DestroyTower();
        }
    }

    void DestroyTower()
    {
        Debug.Log("Tower has been destroyed!");
        Destroy(gameObject);
    }

    public void ApplyUpgrade(UpgradeData upgradeData)
    {
        towerHealth += upgradeData.healthIncrease;
        attackDamage += upgradeData.defenseIncrease;
        attackCooldown -= upgradeData.attackSpeedIncrease;

        if (upgradeData.upgradedPrefab != null)
        {
            Instantiate(upgradeData.upgradedPrefab, transform.position, transform.rotation);
            Destroy(gameObject);  // Replace with upgraded prefab
        }
    }
}
