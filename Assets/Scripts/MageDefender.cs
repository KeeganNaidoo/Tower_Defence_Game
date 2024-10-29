using UnityEngine;

public class MageDefender : Defender
{
    public float aoeDamage = 10f;  // Damage applied in AoE attack

    protected override void Attack()
    {
        // Find all enemies within the attack range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(aoeDamage);  // Apply damage to each enemy in range
            }
        }

        // Optional: Add visual effect to represent the AoE attack
        ShowAoEEffect();
    }

    // Optional function for visual feedback
    void ShowAoEEffect()
    {
        // Assumes a particle system is attached to the Mage Defender
        ParticleSystem aoeEffect = GetComponent<ParticleSystem>();
        if (aoeEffect != null)
        {
            aoeEffect.Play();
        }
    }
}
