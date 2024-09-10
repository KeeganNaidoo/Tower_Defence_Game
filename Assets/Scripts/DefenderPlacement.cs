using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderPlacement : MonoBehaviour
{
    [Header("Defender Prefab")]
    public GameObject defenderPrefab;  // Prefab for the defender to place

    [Header("References")]
    public Camera mainCamera;           // Reference to the main camera
    public TerrainGenerator terrainGenerator;  // Reference to the TerrainGenerator script to get platform positions

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();  // Track occupied platforms
    private const float platformHeight = 3f; // Height of the platform
    private const float tolerance = 1f;  // Tolerance for position comparison

    void Start()
    {
        // Initialize the camera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Get already occupied positions from the TerrainGenerator
        occupiedPositions = terrainGenerator.occupiedPositions;
    }

    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            PlaceDefender();
        }
    }

    void PlaceDefender()
    {
        // Cast a ray from the camera to where the player clicked
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 2f);  // Visible for 2 seconds

        // If the ray hits something
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 clickedPosition = hit.point;
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(clickedPosition.x),
                platformHeight,  // This remains the same for checking valid platforms
                Mathf.Round(clickedPosition.z)
            );

            // Debug: Log the position clicked and rounded
            Debug.Log($"Raycast hit at position: {clickedPosition}");
            Debug.Log($"Rounded platform position: {roundedPosition}");

            // Check if the clicked position is a valid platform
            if (IsPositionValid(roundedPosition))
            {
                // Debug: Valid platform message
                Debug.Log("Valid platform detected!");

                // Check if the platform is already occupied by a defender
                if (!occupiedPositions.Contains(roundedPosition))
                {
                    // Add a slight height offset to ensure the defender is above the platform
                    Vector3 placementPosition = new Vector3(
                        roundedPosition.x,
                        platformHeight + 2.0f,  // Adjust Y-axis to place the defender slightly above the platform
                        roundedPosition.z
                    );

                    // Place a defender on the platform
                    Instantiate(defenderPrefab, placementPosition, Quaternion.identity);

                    // Mark this platform as occupied
                    occupiedPositions.Add(roundedPosition);

                    // Debug: Defender placed message
                    Debug.Log("Defender placed!");
                }
                else
                {
                    Debug.Log("Platform already occupied by a defender.");
                }
            }
            else
            {
                Debug.Log("Invalid placement. You must place defenders on the designated platforms.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object.");
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check if the position is within a tolerance of any occupied position
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

    // The IsPlatformOccupied method is no longer needed as we are tracking positions in occupiedPositions set
}
