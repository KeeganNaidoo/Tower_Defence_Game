using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowSpawner : MonoBehaviour
{
    public GameObject snowflakePrefab; // Assign the snowflake prefab
    public float spawnRate = 0.001f;     // Time between spawns
    public Vector3 spawnArea = new Vector3(150f, 10f, 150f); // X and Z spawn area size
    public float fallSpeed = 5f;       // Speed of snowflake fall
    public float lifetime = 5f;       // Lifetime of each snowflake

    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnSnowflake();
            timer = 0f;
        }
    }
    
    void SpawnSnowflake()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            spawnArea.y,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject snowflake = Instantiate(snowflakePrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));
        snowflake.transform.localScale *= Random.Range(0.5f, 1.5f);

        Rigidbody rb = snowflake.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(0, -fallSpeed, 0);
        }

        Destroy(snowflake, lifetime);
        
        //rb.velocity = new Vector3(Random.Range(-1f, 1f), -fallSpeed, Random.Range(-1f, 1f));
    }
}
