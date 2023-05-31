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
    public float gridSpacing; // This will determine the space between the grids.
    public Vector2Int currentGridPosition = Vector2Int.zero;

    public List<PlantedAreaController> allPlantedAreas; // Tüm PlantedArea'lar

    public int areasPerGroup = 18;
    private int currentAreaGroup = 0;
    private float gridOffset = 0;
    public float OFFSET_BETWEEN_GROUPS = 9.3f;

    public bool isSecondArea;
    public float sayiaq;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allPlantedAreas = new List<PlantedAreaController>();
        gridOffset = isSecondArea ? sayiaq * (currentAreaGroup + 1) : 0; // if isSecondArea is true, gridOffset will be 30 * (currentAreaGroup + 1), otherwise 0
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
        // Eğer iki grup oluşturulmuşsa, daha fazla oluşturma
        if (currentAreaGroup >= 2)
        {
            Debug.Log("Two groups of areas have been created. No more areas will be created.");
            UIManager.Instance.ShowAddNewLandButton();
            return;
        }

        int gridRows = Mathf.FloorToInt(gridSize.x / cellSize); //satir
        int gridColumns = Mathf.FloorToInt(gridSize.z / cellSize); //sutun

        if (allPlantedAreas.Count >= gridRows * gridColumns * (currentAreaGroup + 1))
        {
            Debug.LogError("Max grid count reached");
            return;
        }

        // Grid konumunu dünya konumuna çevir
        Vector3 spawnPosition = new Vector3(currentGridPosition.x * cellSize - gridSize.x / 2 + cellSize / 2 + gridOffset, 0, currentGridPosition.y * cellSize - gridSize.z / 2 + cellSize / 2);

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

            // If we've filled a group of areas, start a new one.
            if (allPlantedAreas.Count >= (currentAreaGroup + 1) * areasPerGroup)
            {
                currentAreaGroup++;
                gridOffset = currentAreaGroup * OFFSET_BETWEEN_GROUPS;
                currentGridPosition = Vector2Int.zero; // reset grid position for new group
            }
        }

        if (allPlantedAreas.Count >= (currentAreaGroup + 1) * areasPerGroup)
        {
            currentAreaGroup++;

            // Increase the offset for the next group only for the second PlantedAreaSpawner
            if (isSecondArea)
            {
                gridOffset += 30;  // Increase gridOffset by 30 for each new group.
            }

            currentGridPosition = Vector2Int.zero; // reset grid position for new group
        }
    }

}