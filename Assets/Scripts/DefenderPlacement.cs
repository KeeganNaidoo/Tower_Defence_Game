using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderPlacement : MonoBehaviour
{
    public GameObject archerDefenderPrefab;
    public GameObject mageDefenderPrefab;
    private GameObject selectedDefenderPrefab;

    public Camera mainCamera;
    public TerrainGenerator terrainGenerator;

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private const float platformHeight = 3f;
    private const float tolerance = 1f;

    void Start()
    {
        // Set default defender to Archer Defender
        if (archerDefenderPrefab != null)
        {
            selectedDefenderPrefab = archerDefenderPrefab;
        }
        else
        {
            Debug.LogError("Archer Defender Prefab is not assigned in the Inspector.");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        occupiedPositions = terrainGenerator.occupiedPositions;
    }

    void Update()
    {
        // Left click to place Archer Defender
        if (Input.GetMouseButtonDown(0))
        {
            selectedDefenderPrefab = archerDefenderPrefab;
            PlaceDefender();
        }

        // Right click to place Mage Defender
        if (Input.GetMouseButtonDown(1))
        {
            selectedDefenderPrefab = mageDefenderPrefab;
            PlaceDefender();
        }

        // Detect defender selection for upgrades
        if (Input.GetMouseButtonDown(0)) // Left-click to select defender for upgrades
        {
            SelectDefenderForUpgrade();
        }
    }

    void PlaceDefender()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 clickedPosition = hit.point;
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(clickedPosition.x),
                platformHeight,
                Mathf.Round(clickedPosition.z)
            );

            if (IsPositionValid(roundedPosition) && !occupiedPositions.Contains(roundedPosition))
            {
                Vector3 placementPosition = new Vector3(
                    roundedPosition.x,
                    platformHeight + 2.0f,
                    roundedPosition.z
                );

                // Instantiate the selected defender prefab
                if (selectedDefenderPrefab != null)
                {
                    GameObject defender = Instantiate(selectedDefenderPrefab, placementPosition, Quaternion.identity);
                    occupiedPositions.Add(roundedPosition);

                    // Add an OnClick method to this defender to select it for upgrades
                    defender.AddComponent<DefenderClickHandler>().InitializeForUpgrade();
                    Debug.Log("Defender placed!");
                }
                else
                {
                    Debug.LogError("Selected defender prefab is not assigned.");
                }
            }
            else
            {
                Debug.Log("Platform already occupied or invalid position.");
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Mathf.Abs(position.x - occupied.x) <= tolerance &&
                Mathf.Abs(position.z - occupied.z) <= tolerance &&
                Mathf.Abs(position.y - occupied.y) <= tolerance)
            {
                return true;
            }
        }
        return false;
    }

    // Raycast to select a defender for upgrades
    void SelectDefenderForUpgrade()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Defender defender = hit.collider.GetComponent<Defender>();
            if (defender != null)
            {
                UpgradeUI upgradeUI = FindObjectOfType<UpgradeUI>();
                if (upgradeUI != null)
                {
                    upgradeUI.SelectDefenderForUpgrade(defender);  // Select the clicked defender for upgrades
                    Debug.Log("Defender selected for upgrades: " + defender.name);
                }
            }
        }
    }
}
