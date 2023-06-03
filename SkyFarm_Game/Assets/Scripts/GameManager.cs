using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Plants[] allPlants;
    public Plants.PlantType selectedPlantType;

    public IntVariable gold;
    public event Action<int> OnGoldChanged;
    public PlayerBehaviour playerBehaviour;
    public JoystickController joystickController;

    public NPCController selectedNPC;

    public int areaValue = 10;
    
    public float moveDuration = 1f;

    public PlantedAreaSpawner firstAreaSpawner;
    public PlantedAreaSpawner secondAreaSpawner;
    public GameObject movingObject;
    public float delayDuration = 2f;
    public bool isNewLandActive = false;

    public bool isNPCEngaged = false;


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
        if ((int)selectedPlantType >= 0 && (int)selectedPlantType < allPlants.Length)
        {
            return allPlants[(int)selectedPlantType];
        }

        Debug.LogError("Invalid plant type: " + selectedPlantType);
        return null;
    }

    public void SwitchChildObject()
    {
        PlayerBehaviour playerBehaviour = GameManager.Instance.playerBehaviour;

        // activeChildIndex ve activeHandIndex değerini artırın ve çocuk objelerin sayısına göre modunu alın
        int newChildIndex = (playerBehaviour.activeChildIndex + 1) % playerBehaviour.childObjectsWithAnimators.Length;

        playerBehaviour.UpgradeCharacter(newChildIndex);

        playerBehaviour.SwitchChildObject(newChildIndex);
        playerBehaviour.SwitchHandObject(newChildIndex);

        joystickController.SwitchChildObject(newChildIndex);
    }

    public void SelectNPC(NPCController npc)
    {
        if (isNPCEngaged) return;

        // Otherwise, select the NPC and update the engaged state
        selectedNPC = npc;
        isNPCEngaged = true;
    }

    public void StartSelectedNPC()
    {
        if (selectedNPC != null)
        {
            selectedNPC.StopCheckingPlants();
        }

        selectedNPC.StartCollecting();

        // After starting the NPC, update the NPC's engaged state
        isNPCEngaged = true;
    }

    public void MoveCameraAndObject()
    {
        // Disable the CamFollow script
        CamFollow camFollow = Camera.main.GetComponent<CamFollow>();
        camFollow.enabled = false;

        // Move the camera back
        Vector3 cameraTargetPosition = Camera.main.transform.position + new Vector3(30, 20, -50); // Modify this to your needs

        Sequence sequence = DOTween.Sequence(); // Create a sequence
        sequence.Append(Camera.main.transform.DOMove(cameraTargetPosition, moveDuration)); // Append move operation
        sequence.AppendInterval(2f); // Append a 2 seconds delay after moving
        sequence.OnComplete(() => camFollow.enabled = true); // Enable the CamFollow script when the sequence completes

        isNewLandActive = true;
        isNPCEngaged = false;

        // Move the new object to the left
        Vector3 objectTargetPosition = movingObject.transform.position + new Vector3(-10, 0, 0); // Modify this to your needs
        movingObject.transform.DOMove(objectTargetPosition, moveDuration);

        UIManager.Instance.currentPlantedAreaSpawner = secondAreaSpawner;
        UIManager.Instance.addNewLandButton.gameObject.SetActive(false);
    }

    public void OnNewLandFullyActive()
    {
        isNewLandActive = false;
    }
}