using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Plants[] allPlants;
    public Plants.PlantType selectedPlantType;

    public IntVariable gold;
    public event Action<int> OnGoldChanged;
    public PlayerBehaviour playerBehaviour;
    public JoystickController joystickController;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeGold(int amount)
    {
        gold.count += amount;
        OnGoldChanged?.Invoke(gold.count);
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

    public void SwitchChildObject()
    {
        // PlayerBehaviour nesnesine bir referans alın
        PlayerBehaviour playerBehaviour = GameManager.Instance.playerBehaviour;

        // activeChildIndex ve activeHandIndex değerini artırın ve çocuk objelerin sayısına göre modunu alın
        int newChildIndex = (playerBehaviour.activeChildIndex + 1) % playerBehaviour.childObjectsWithAnimators.Length;

        // Karakteri yükseltin
        playerBehaviour.UpgradeCharacter(newChildIndex);

        // Çocuk objesini ve el objesini değiştirin
        playerBehaviour.SwitchChildObject(newChildIndex);
        playerBehaviour.SwitchHandObject(newChildIndex);  // Hand objesini de değiştirin

        // JoystickController çocuk nesnesini de değiştirin
        joystickController.SwitchChildObject(newChildIndex);
    }

}
