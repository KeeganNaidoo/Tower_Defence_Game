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
                platformHeight,
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
                if (!IsPlatformOccupied(roundedPosition))
                {
                    // Place a defender on the platform
                    Instantiate(defenderPrefab, roundedPosition, Quaternion.identity);

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

    bool IsPlatformOccupied(Vector3 position)
    {
        // Cast a small sphere to check if there's already a defender on the platform
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f); // Adjust radius as needed
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Defender"))
            {
                return true; // Platform is already occupied
            }
        }
        return false; // Platform is free for placement
    }
}
