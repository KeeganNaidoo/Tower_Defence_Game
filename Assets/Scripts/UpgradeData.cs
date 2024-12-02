using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public float healthIncrease;
    public float attackSpeedIncrease;
    public float defenseIncrease;
    public GameObject upgradedPrefab;  // Optional: Prefab for upgraded appearance
}
