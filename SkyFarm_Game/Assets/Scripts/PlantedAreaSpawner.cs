using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedAreaSpawner : MonoBehaviour
{
    public static PlantedAreaSpawner Instance;

    public GameObject plantedAreaPrefab; // PlantedArea prefab'ınız
    public int initialAreaCount;
    public Vector3 gridSize;
    public float cellSize;
    public Vector2Int currentGridPosition = Vector2Int.zero;

    public List<PlantedAreaController> allPlantedAreas; // Tüm PlantedArea'lar

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allPlantedAreas = new List<PlantedAreaController>();
        SpawnInitialAreas();
    }

    void SpawnInitialAreas()
    {
        for (int i = 0; i < initialAreaCount; i++)
        {
            SpawnPlantedArea();
        }
    }

    public void SpawnPlantedArea()
    {
        int gridRows = Mathf.FloorToInt(gridSize.x / cellSize); //satir
        int gridColumns = Mathf.FloorToInt(gridSize.z / cellSize); //sutun

        if (allPlantedAreas.Count >= gridRows * gridColumns)
        {
            Debug.LogError("grid sayisi alan sayisindan az");
            return;
        }

        // Grid konumunu dünya konumuna çevir
        Vector3 spawnPosition = new Vector3(currentGridPosition.x * cellSize - gridSize.x / 2 + cellSize / 2, 0, currentGridPosition.y * cellSize - gridSize.z / 2 + cellSize / 2);

        GameObject newArea = Instantiate(plantedAreaPrefab, spawnPosition, Quaternion.identity, transform);
        PlantedAreaController areaController = newArea.GetComponent<PlantedAreaController>();
        if (areaController != null)
        {
            if (!allPlantedAreas.Contains(areaController))
            {
                allPlantedAreas.Add(areaController);
            }
        }

        // Grid konumunu güncelle
        if (currentGridPosition.x < gridRows - 1)
        {
            currentGridPosition.x++;
        }
        else
        {
            currentGridPosition.x = 0;
            currentGridPosition.y++;
        }
    }
}
