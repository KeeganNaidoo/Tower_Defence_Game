using UnityEngine;

public class DefenderClickHandler : MonoBehaviour
{
    private Defender defender;

    public void InitializeForUpgrade()
    {
        defender = GetComponent<Defender>();
    }

    void OnMouseDown()
    {
        UpgradeUI upgradeUI = FindObjectOfType<UpgradeUI>();
        if (upgradeUI != null)
        {
            upgradeUI.SelectDefenderForUpgrade(defender);  // Set the clicked defender as the selected one for upgrades
        }
    }
}
