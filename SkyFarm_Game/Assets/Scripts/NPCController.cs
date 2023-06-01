using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject targetPlant;
    private bool isCollecting = false;
    private bool isSelling = false;
    [SerializeField] private List<Plants> carriedPlants = new List<Plants>();

    public Transform salesArea;
    [SerializeField] private int maxPlantCarryCapacity = 1;
    [SerializeField] private int currentPlantCarryCount = 0;
    private int plantValue = 0;
    public Transform startLocation;

    private Coroutine checkPlantsCoroutine;

    //public Animator animator;

    public GameObject[] childObjects;
    public Animator[] childAnimators;

    public GameObject npcUiElement;
    public TextMeshProUGUI speedText;

    public Button startCollectingButton;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent attached to " + gameObject.name);
        }
        //maxPlantCarryCapacity = Random.Range(2, 5);
        agent.speed = Random.Range(3, 6);
        Debug.Log(maxPlantCarryCapacity);
        //startLocation = transform.position;

        //animator = GetComponent<Animator>();

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(i == 0); // Sadece ilk objeyi etkinleştir
            childAnimators[i] = childObjects[i].GetComponent<Animator>();
        }

        npcUiElement.SetActive(false);

        speedText.text = agent.speed.ToString();

        NPCCC();
    }

    public void NPCCC()
    {
        startCollectingButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SelectNPC(this);
            GameManager.Instance.StartSelectedNPC();
        });
    }

    public void StartCollecting()
    {
        if (checkPlantsCoroutine == null)
        {
            checkPlantsCoroutine = StartCoroutine(CheckPlants());
        }
    }

    public void StartCheckingPlants()
    {
        if (checkPlantsCoroutine == null)
        {
            checkPlantsCoroutine = StartCoroutine(CheckPlants());
        }
    }

    public void StopCheckingPlants()
    {
        if (checkPlantsCoroutine != null)
        {
            StopCoroutine(checkPlantsCoroutine);
            checkPlantsCoroutine = null;
        }
    }

    private IEnumerator CheckPlants()
    {
        while (true)
        {
            if (GameManager.Instance.selectedNPC == this && !GameManager.Instance.isNewLandActive)
            {
                int grownPlantsCount = CountGrownPlants();

                if (grownPlantsCount >= maxPlantCarryCapacity)
                {
                    if (!isCollecting && !isSelling)
                    {
                        FindPlant();
                    }
                }
            }

            yield return new WaitForSeconds(1);
        }
    }


    private void Update()
    {
        if (GameManager.Instance.isNewLandActive) return;

        if (GameManager.Instance.selectedNPC != this) return;

        int grownPlantsCount = CountGrownPlants();

        if (currentPlantCarryCount >= maxPlantCarryCapacity)
        {
            isSelling = true;
        }

        if (grownPlantsCount >= maxPlantCarryCapacity)
        {
            if (isSelling)
            {
                agent.SetDestination(salesArea.position);
                if (Vector3.Distance(transform.position, salesArea.position) < agent.stoppingDistance)
                {
                    SellPlant();
                }
            }
            else if (!isCollecting)
            {
                FindPlant();
            }
        }
        else if (isSelling)
        {
            agent.SetDestination(salesArea.position);
        }
        else if (isCollecting)
        {
            if (targetPlant != null)
            {
                if (currentPlantCarryCount < maxPlantCarryCapacity)
                {
                    agent.SetDestination(targetPlant.transform.position);
                }
                else
                {
                    isCollecting = false;
                    agent.SetDestination(startLocation.position);
                }
            }
            else
            {
                isCollecting = false;
                agent.SetDestination(startLocation.position);
            }
        }

        if (!isCollecting && !isSelling && Vector3.Distance(transform.position, startLocation.position) < agent.stoppingDistance)
        {
            // Set the rotation
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log("NPC is rotating");
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant") && !isSelling)
        {
            PlantController plantController = other.GetComponent<PlantController>();
            if (plantController != null && plantController.isGrown && !plantController.isCollected)
            {
                CollectPlant(plantController);
            }
        }
        else if (other.CompareTag("SalesArea") && isSelling)
        {
            SellPlant();
        }

        if (other.gameObject.CompareTag("Player"))
        {
            // Show the UI element
            npcUiElement.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            npcUiElement.SetActive(false);
        }
    }

    private void FindPlant()
    {
        if (GameManager.Instance.selectedNPC != this) return;

        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        float closestDistance = Mathf.Infinity;
        GameObject closestPlant = null;
        foreach (GameObject plant in plants)
        {
            PlantController plantController = plant.GetComponent<PlantController>();
            if (plantController != null && plantController.isGrown && !plantController.isCollected)
            {
                float distance = Vector3.Distance(transform.position, plant.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlant = plant;
                }
            }
        }

        if (closestPlant != null)
        {
            targetPlant = closestPlant;
            isCollecting = true;
            agent.SetDestination(targetPlant.transform.position);
        }
        else
        {
            targetPlant = null;
            isCollecting = false;
        }
    }

    private void CollectPlant(PlantController plantController)
    {
        if (GameManager.Instance.selectedNPC != this) return;

        int grownPlantsCount = CountGrownPlants();
        if (grownPlantsCount >= maxPlantCarryCapacity && !plantController.isCollected)
        {
            plantValue += plantController.plants.value;
            carriedPlants.Add(plantController.plants);
            Destroy(plantController.gameObject);
            currentPlantCarryCount++;

            plantController.isCollected = true;
            plantController.isGrown = false;

            carriedPlants.Add(plantController.plants);

            for (int i = 0; i < childObjects.Length; i++)
            {
                childObjects[i].SetActive(i == plantController.plants.childIndex);
            }

            if (currentPlantCarryCount >= maxPlantCarryCapacity)
            {
                isCollecting = false;
                isSelling = true;
            }

            else
            {
                grownPlantsCount = CountGrownPlants();

                if (grownPlantsCount >= maxPlantCarryCapacity)
                {
                    FindPlant();
                }
                else
                {
                    agent.SetDestination(startLocation.position);
                    isCollecting = false;
                }
            }
        }
        else
        {
            agent.SetDestination(startLocation.position);
            isCollecting = false;
        }
    }

    private int CountGrownPlants()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        int grownPlantsCount = 0;
        foreach (GameObject plant in plants)
        {
            PlantController plantCtrl = plant.GetComponent<PlantController>();
            if (plantCtrl != null && plantCtrl.isGrown)
            {
                grownPlantsCount++;
            }
        }
        return grownPlantsCount;
    }

    private void SellPlant()
    {
        if (isSelling)
        {
            foreach (var plant in carriedPlants)
            {
                GameManager.Instance.gold.count += plant.value;
                GameManager.Instance.ChangeGold(plant.value);
            }

            currentPlantCarryCount = 0;
            plantValue = 0;
            carriedPlants.Clear();

            isSelling = false;

            for (int i = 0; i < childObjects.Length; i++)
            {
                childObjects[i].SetActive(i == 0); // Sadece ilk objeyi etkinleştir
            }

            if (CountGrownPlants() >= maxPlantCarryCapacity)
            {
                FindPlant();
            }
            else
            {
                agent.SetDestination(startLocation.position);
            }
        }
    }
}