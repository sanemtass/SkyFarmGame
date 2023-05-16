using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Plants[] allPlants;

    public Plants.PlantType selectedPlantType;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectPlant1()
    {
        selectedPlantType = Plants.PlantType.Plant1;
    }

    public void SelectPlant2()
    {
        selectedPlantType = Plants.PlantType.Plant2;
    }

    public void SelectPlant3()
    {
        selectedPlantType = Plants.PlantType.Plant3;
    }

    public Plants GetSelectedPlant()
    {
        // Ensure the selected plant type is a valid index in the array
        if ((int)selectedPlantType >= 0 && (int)selectedPlantType < allPlants.Length)
        {
            return allPlants[(int)selectedPlantType];
        }

        Debug.LogError("Invalid plant type: " + selectedPlantType);
        return null;
    }
}
