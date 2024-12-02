using UnityEngine;

public class Upgrade
{
    public float healthIncrease;
    public float attackSpeedIncrease;
    public float defenseIncrease;
    public GameObject upgradedPrefab;

    public Upgrade(float health, float speed, float defense, GameObject prefab)
    {
        healthIncrease = health;
        attackSpeedIncrease = speed;
        defenseIncrease = defense;
        upgradedPrefab = prefab;
    }
}
