using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject snowBlockPrefab;
    public GameObject iceBlockPrefab;
    public GameObject walkableBlockPrefab;
    public GameObject mainTowerPrefab;
    public GameObject iceTreePrefab;
    public GameObject snowTreePrefab;
    public GameObject placeableBlockPrefab; // Prefab for placeable defender blocks
    public GameObject nonPlaceableBlockPrefab; // Prefab for non-placeable defender blocks
    public int gridSize = 30; // 30x30 grid
    public float blockSize = 1.0f; // Size of each block
    public int numberOfSnowTrees = 10; // Changeable number of snow trees to place
    public int numberOfIceTrees = 5;  // Changeable number of ice trees to place

    private bool[,] reservedGrid; // Grid to mark reserved areas (paths + border)
    private List<Vector3> snowBlockPositions = new List<Vector3>(); // Store snow block positions for placing snow trees
    private List<Vector3> iceBlockPositions = new List<Vector3>();  // Store ice block positions for placing ice trees

    private void Start()
    {
        reservedGrid = new bool[gridSize, gridSize]; // Initialize grid to track reserved blocks
        GenerateSquareGrid();
    }

    // Generates the terrain and paths
    void GenerateSquareGrid()
    {
        int centerX = gridSize / 2;
        int centerZ = gridSize / 2;

        // Create the main tower in the center
        Vector3 towerPosition = new Vector3(centerX * blockSize, 0, centerZ * blockSize);
        Instantiate(mainTowerPrefab, towerPosition, Quaternion.identity, transform);

        // Define the 3 path end points (equally spaced on the grid's edge)
        Vector3[] pathEndPoints = new Vector3[3];

        // First path: North (Z+)
        pathEndPoints[0] = new Vector3(centerX * blockSize, 0, (gridSize - 1) * blockSize);
        
        // Second path: East (Right)
        pathEndPoints[1] = new Vector3((gridSize - 1) * blockSize, 0, centerZ * blockSize);
        
        // Third path: West (Left)
        pathEndPoints[2] = new Vector3(0, 0, centerZ * blockSize);

        // Generate paths from the center (main tower) to the 3 edge points
        for (int i = 0; i < pathEndPoints.Length; i++)
        {
            GeneratePath(new Vector3(centerX * blockSize, 0, centerZ * blockSize), pathEndPoints[i]);
        }

        // Generate the rest of the terrain
        GenerateTerrain(gridSize);

        // Generate the raised border between terrain and paths
        GenerateBorder();

        // Place snow and ice trees
        PlaceSnowTrees();
        PlaceIceTrees();
    }

    // Generates a straight path from start to end and reserves a border around it
    void GeneratePath(Vector3 start, Vector3 end)
    {
        int pathWidth = 2; // Width of each path

        // Direction and distance
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // Create the path along the calculated direction
        for (float i = 0; i <= distance; i += blockSize)
        {
            Vector3 pathPosition = start + direction * i;

            for (int j = -pathWidth / 2; j <= pathWidth / 2; j++)
            {
                Vector3 offset = Vector3.Cross(direction, Vector3.up) * j * blockSize;
                Vector3 blockPosition = pathPosition + offset;

                // Remove any existing terrain block at this position
                Collider[] hitColliders = Physics.OverlapBox(blockPosition, Vector3.one * (blockSize / 2));
                foreach (var hitCollider in hitColliders)
                {
                    Destroy(hitCollider.gameObject);
                }

                // Lower the path by 0.3 units on the Y axis
                blockPosition.y -= 0.3f;

                // Place walkable path block
                Instantiate(walkableBlockPrefab, blockPosition, Quaternion.identity, transform);

                // Mark path blocks as reserved
                reservedGrid[(int)(blockPosition.x / blockSize), (int)(blockPosition.z / blockSize)] = true;
            }
        }
    }

    // Generates the raised border between the terrain and paths
    void GenerateBorder()
    {
        int borderWidth = 2; // Width of the border around paths

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Only place border blocks between terrain and paths
                if (reservedGrid[x, z] == false && IsBorder(x, z, borderWidth))
                {
                    Vector3 position = new Vector3(x * blockSize, 0.5f, z * blockSize); // Raised by 0.5 units

                    // Alternate between placeable and non-placeable blocks in a 2x2 pattern
                    if ((x / 2 + z / 2) % 2 == 0)
                    {
                        Instantiate(placeableBlockPrefab, position, Quaternion.identity, transform);
                    }
                    else
                    {
                        Instantiate(nonPlaceableBlockPrefab, position, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    // Check if a block is part of the border
    bool IsBorder(int x, int z, int borderWidth)
    {
        for (int dx = -borderWidth; dx <= borderWidth; dx++)
        {
            for (int dz = -borderWidth; dz <= borderWidth; dz++)
            {
                if (x + dx >= 0 && x + dx < gridSize && z + dz >= 0 && z + dz < gridSize)
                {
                    if (reservedGrid[x + dx, z + dz] == true)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Generates the rest of the terrain (snow and ice blocks)
    void GenerateTerrain(int gridSize)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(x * blockSize, 0, z * blockSize);

                // Skip terrain generation if it's reserved for paths or borders
                if (reservedGrid[x, z])
                    continue;

                // Randomly select between snow and ice blocks (more snow than ice)
                GameObject blockPrefab = Random.value < 0.8f ? snowBlockPrefab : iceBlockPrefab; // 80% snow, 20% ice

                // Instantiate block
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, transform);

                if (blockPrefab == iceBlockPrefab)
                {
                    // Store the position for ice tree placement later
                    iceBlockPositions.Add(position);
                }
                else
                {
                    // Store the position for snow tree placement later
                    snowBlockPositions.Add(position);
                }
            }
        }
    }

    // Places a specified number of snow trees on random snow blocks
    void PlaceSnowTrees()
    {
        // Shuffle the snow block positions to randomize tree placement
        for (int i = 0; i < snowBlockPositions.Count; i++)
        {
            Vector3 temp = snowBlockPositions[i];
            int randomIndex = Random.Range(i, snowBlockPositions.Count);
            snowBlockPositions[i] = snowBlockPositions[randomIndex];
            snowBlockPositions[randomIndex] = temp;
        }

        // Place snow trees on the first 'numberOfSnowTrees' snow blocks
        for (int i = 0; i < Mathf.Min(numberOfSnowTrees, snowBlockPositions.Count); i++)
        {
            Vector3 treePosition = new Vector3(snowBlockPositions[i].x, snowBlockPositions[i].y + blockSize, snowBlockPositions[i].z);
            Instantiate(snowTreePrefab, treePosition, Quaternion.identity, transform);
        }
    }

    // Places a specified number of ice trees on random ice blocks
    void PlaceIceTrees()
    {
        // Shuffle the ice block positions to randomize tree placement
        for (int i = 0; i < iceBlockPositions.Count; i++)
        {
            Vector3 temp = iceBlockPositions[i];
            int randomIndex = Random.Range(i, iceBlockPositions.Count);
            iceBlockPositions[i] = iceBlockPositions[randomIndex];
            iceBlockPositions[randomIndex] = temp;
        }

        // Place ice trees on the first 'numberOfIceTrees' ice blocks
        for (int i = 0; i < Mathf.Min(numberOfIceTrees, iceBlockPositions.Count); i++)
        {
            Vector3 treePosition = new Vector3(iceBlockPositions[i].x, iceBlockPositions[i].y + blockSize, iceBlockPositions[i].z);
            Instantiate(iceTreePrefab, treePosition, Quaternion.identity, transform);
        }
    }
}