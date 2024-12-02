using UnityEngine;
using UnityEngine.UI;  // For UI elements

public class Defender : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    public float projectileSpeed = 20f;
    public float rotationSpeed = 5f;
    public float hp = 50f;
    public float damage = 10f; // Added damage stat
    public float attackSpeed = 1f;  // Attack speed stat

    // UI Elements to display stats
    public Text defenderHealthText;
    public Text defenderAttackSpeedText;
    public Text defenderDamageText;

    private float attackCooldownTimer;
    private Transform currentTarget;

    void Start()
    {
        // Display initial stats in UI
        UpdateDefenderStatsUI();
    }

    void Update()
    {
        FindTarget();
        if (currentTarget != null)
        {
            RotateTowards(currentTarget);
            if (attackCooldownTimer <= 0f)
            {
                Attack();
                attackCooldownTimer = attackCooldown;
            }
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    protected virtual void Attack()
    {
        ShootProjectile(currentTarget);
    }

    void ShootProjectile(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(target, projectileSpeed);
        }
    }

    void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
        UpdateDefenderStatsUI();  // Update UI after taking damage
    }

    void FindTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                currentTarget = enemy.transform;
                break;
            }
        }
    }

    public void ApplyUpgrade(UpgradeData upgradeData)
    {
        hp += upgradeData.healthIncrease;
        attackCooldown -= upgradeData.attackSpeedIncrease;
        damage += upgradeData.defenseIncrease;  // Update damage based on upgrade
        if (upgradeData.upgradedPrefab != null)
        {
            Instantiate(upgradeData.upgradedPrefab, transform.position, transform.rotation);
            Destroy(gameObject);  // Replace with upgraded prefab
        }

        UpdateDefenderStatsUI();  // Update UI after upgrade
    }

    // Update the displayed stats on the UI
    void UpdateDefenderStatsUI()
    {
        if (defenderHealthText != null)
            defenderHealthText.text = "Health: " + hp.ToString();

        if (defenderAttackSpeedText != null)
            defenderAttackSpeedText.text = "Attack Speed: " + attackSpeed.ToString("F2");

        if (defenderDamageText != null)
            defenderDamageText.text = "Damage: " + damage.ToString("F2");
    }
}
