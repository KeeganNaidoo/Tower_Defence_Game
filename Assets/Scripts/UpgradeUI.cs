using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Defender selectedDefender;  // The defender selected for upgrades
    public Tower mainTower;
    public UpgradeData[] defenderUpgrades;  // ScriptableObjects for defender upgrades
    public UpgradeData[] towerUpgrades;     // ScriptableObjects for tower upgrades

    public Button[] defenderUpgradeButtons;  // UI Buttons for defender upgrades
    public Button[] towerUpgradeButtons;     // UI Buttons for tower upgrades
    public Text upgradeStatusText;   // UI Text to display upgrade status (optional)

    void Start()
    {
        // Set up each defender upgrade button with the correct upgrade level
        for (int i = 0; i < defenderUpgradeButtons.Length; i++)
        {
            int index = i;  // Capture the index for the current button
            defenderUpgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(index, "defender"));  // Associate each button with its defender upgrade level
        }

        // Set up each tower upgrade button with the correct upgrade level
        for (int i = 0; i < towerUpgradeButtons.Length; i++)
        {
            int index = i;  // Capture the index for the current button
            towerUpgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(index, "tower"));  // Associate each button with its tower upgrade level
        }
    }

    public void SelectDefenderForUpgrade(Defender defender)
    {
        // Set the selected defender for upgrades
        selectedDefender = defender;
    }

    public void ApplyUpgrade(int upgradeLevel, string upgradeType)
    {
        bool upgradeApplied = false;

        if (upgradeType == "defender" && selectedDefender != null && upgradeLevel < defenderUpgrades.Length)
        {
            // Apply defender upgrade
            selectedDefender.ApplyUpgrade(defenderUpgrades[upgradeLevel]);
            upgradeApplied = true;
            Debug.Log("Defender upgrade applied: " + defenderUpgrades[upgradeLevel].name);  // Log message
        }
        else if (upgradeType == "tower" && mainTower != null && upgradeLevel < towerUpgrades.Length)
        {
            // Apply tower upgrade
            mainTower.ApplyUpgrade(towerUpgrades[upgradeLevel]);
            upgradeApplied = true;
            Debug.Log("Tower upgrade applied: " + towerUpgrades[upgradeLevel].name);  // Log message
        }

        // Optional: Update the UI to show the upgrade status
        if (upgradeApplied)
        {
            if (upgradeStatusText != null)
            {
                upgradeStatusText.text = "Upgrade Applied!";
            }
        }
        else
        {
            if (upgradeStatusText != null)
            {
                upgradeStatusText.text = "Upgrade Failed!";
            }
        }
    }
}
