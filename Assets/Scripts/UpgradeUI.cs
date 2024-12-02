using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Defender selectedDefender;  // The defender selected for upgrades
    public Tower mainTower;
    public UpgradeData[] archerUpgrades;  // ScriptableObjects for Archer upgrades
    public UpgradeData[] mageUpgrades;    // ScriptableObjects for Mage upgrades
    public UpgradeData[] towerUpgrades;   // ScriptableObjects for Tower upgrades

    public Button[] archerUpgradeButtons;  // UI Buttons for Archer upgrades
    public Button[] mageUpgradeButtons;    // UI Buttons for Mage upgrades
    public Button[] towerUpgradeButtons;   // UI Buttons for Tower upgrades
    public Text upgradeStatusText;         // UI Text to display upgrade status (optional)

    // UI Text elements for displaying defender stats
    public Text defenderHealthText;
    public Text defenderAttackSpeedText;
    public Text defenderDamageText;

    // UI Text elements for displaying tower stats
    public Text towerHealthText;
    public Text towerDamageText;

    void Start()
    {
        // Set up buttons for Archer upgrades
        for (int i = 0; i < archerUpgradeButtons.Length; i++)
        {
            int index = i;
            archerUpgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(index, "archer"));
        }

        // Set up buttons for Mage upgrades
        for (int i = 0; i < mageUpgradeButtons.Length; i++)
        {
            int index = i;
            mageUpgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(index, "mage"));
        }

        // Set up buttons for Tower upgrades
        for (int i = 0; i < towerUpgradeButtons.Length; i++)
        {
            int index = i;
            towerUpgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(index, "tower"));
        }

        // Disable all upgrade buttons initially
        DisableAllUpgradeButtons();
    }

    // Select the defender for upgrades
    public void SelectDefenderForUpgrade(Defender defender)
    {
        selectedDefender = defender;
        Debug.Log("Defender selected for upgrades: " + defender.name);

        // Update UI for the selected defender's stats
        UpdateDefenderStatsUI();

        // Enable relevant upgrade buttons based on the defender type
        DisableAllUpgradeButtons();
        if (defender is ArcherDefender)  // Archer defender selected
        {
            EnableButtons(archerUpgradeButtons);
        }
        else if (defender is MageDefender)  // Mage defender selected
        {
            EnableButtons(mageUpgradeButtons);
        }
    }

    // Update UI to display the selected defender's stats
    void UpdateDefenderStatsUI()
    {
        if (selectedDefender != null)
        {
            defenderHealthText.text = "Health: " + selectedDefender.hp.ToString();
            defenderAttackSpeedText.text = "Attack Speed: " + selectedDefender.attackSpeed.ToString("F2");
            defenderDamageText.text = "Damage: " + selectedDefender.damage.ToString("F2");
        }
    }

    // Update UI to display the tower's stats
    void UpdateTowerStatsUI()
    {
        if (mainTower != null)
        {
            towerHealthText.text = "Health: " + mainTower.towerHealth.ToString();
            towerDamageText.text = "Damage: " + mainTower.attackDamage.ToString("F2");
        }
    }

    // Enable the appropriate upgrade buttons
    void EnableButtons(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;  // Enable the buttons for the selected defender type
        }
    }

    // Disable all upgrade buttons
    void DisableAllUpgradeButtons()
    {
        foreach (Button button in archerUpgradeButtons)
        {
            button.interactable = false;  // Disable Archer upgrade buttons
        }

        foreach (Button button in mageUpgradeButtons)
        {
            button.interactable = false;  // Disable Mage upgrade buttons
        }

        foreach (Button button in towerUpgradeButtons)
        {
            button.interactable = true;  // Tower upgrades are always available
        }
    }

    // Apply the selected upgrade based on the defender type
    public void ApplyUpgrade(int upgradeLevel, string defenderType)
    {
        bool upgradeApplied = false;

        if (defenderType == "archer" && selectedDefender != null && upgradeLevel < archerUpgrades.Length)
        {
            selectedDefender.ApplyUpgrade(archerUpgrades[upgradeLevel]);
            upgradeApplied = true;
            Debug.Log("Archer upgrade applied: " + archerUpgrades[upgradeLevel].name);  // Log message
        }
        else if (defenderType == "mage" && selectedDefender != null && upgradeLevel < mageUpgrades.Length)
        {
            selectedDefender.ApplyUpgrade(mageUpgrades[upgradeLevel]);
            upgradeApplied = true;
            Debug.Log("Mage upgrade applied: " + mageUpgrades[upgradeLevel].name);  // Log message
        }
        else if (defenderType == "tower" && mainTower != null && upgradeLevel < towerUpgrades.Length)
        {
            mainTower.ApplyUpgrade(towerUpgrades[upgradeLevel]);
            upgradeApplied = true;
            Debug.Log("Tower upgrade applied: " + towerUpgrades[upgradeLevel].name);  // Log message
        }

        // Update stats UI after upgrade
        if (selectedDefender != null)
        {
            UpdateDefenderStatsUI();
        }
        if (mainTower != null)
        {
            UpdateTowerStatsUI();
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
