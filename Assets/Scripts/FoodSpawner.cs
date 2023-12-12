using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public Vector3 spawnAreaCenter = new Vector3(0f, 0f, 0f);
    public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

    private void Awake()
    {
        // Spawn food immediately on Awake
        SpawnFood();
    }

    private void SpawnFood()
    {
        // Calculate a random position within the spawn area
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Instantiate the food prefab at the random position
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Calculate random x and z coordinates within the spawn area
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2);

        // Use the spawnAreaCenter's y coordinate as the y coordinate for the spawn position
        float spawnY = spawnAreaCenter.y;

        return new Vector3(randomX, spawnY, randomZ);
    }
}