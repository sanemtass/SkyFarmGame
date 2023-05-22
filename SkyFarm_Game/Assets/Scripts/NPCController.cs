using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject targetPlant;
    private bool isCollecting = false;
    private bool isSelling = false;
    [SerializeField] private List<Plants> carriedPlants = new List<Plants>();  // NPC'nin taşıdığı bitkilerin listesi

    public Transform salesArea;
    [SerializeField] private int maxPlantCarryCapacity;
    private int currentPlantCarryCount = 0;
    private int plantValue = 0;
    public Vector3 startLocation;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent attached to " + gameObject.name);
        }
        maxPlantCarryCapacity = Random.Range(2, 5);
        agent.speed = Random.Range(3, 6); // Bu satır NPC'nin hızını rastgele bir değerle ayarlar.
        Debug.Log(maxPlantCarryCapacity);
        startLocation = transform.position;
    }

    private void Update()
    {
        int grownPlantsCount = CountGrownPlants();

        if (isSelling)
        {
            agent.SetDestination(salesArea.position);
            if (Vector3.Distance(transform.position, salesArea.position) < agent.stoppingDistance)
            {
                SellPlant();
            }
        }
        else if (!isCollecting && grownPlantsCount >= maxPlantCarryCapacity) // Eğer bir bitki toplanmak üzere işaretlenmemişse ve olgunlaşmış bitki sayısı taşıma kapasitesinden fazla veya eşitse, yeni bir bitki ara
        {
            FindPlant();
        }
        else if (isCollecting)
        {
            if (targetPlant != null)
            {
                if (currentPlantCarryCount < maxPlantCarryCapacity && grownPlantsCount >= maxPlantCarryCapacity)
                {
                    // Yeterli sayıda bitki olgunlaştıysa, toplamayı devam et
                    agent.SetDestination(targetPlant.transform.position);
                }
                else
                {
                    // Aksi takdirde toplamayı durdur ve başlangıç konumuna dön
                    isCollecting = false;
                    agent.SetDestination(startLocation);
                }
            }
            else
            {
                // targetPlant null olduğunda isCollecting durumunu false yap ve başlangıç konumuna dön
                isCollecting = false;
                agent.SetDestination(startLocation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant") && !isSelling)
        {
            PlantController plantController = other.GetComponent<PlantController>();

            if (plantController != null && plantController.isGrown)
            {
                CollectPlant(plantController);
            }
        }

        else if (other.CompareTag("SalesArea") && isSelling)
        {
            SellPlant();
        }
    }

    private void FindPlant()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        float closestDistance = Mathf.Infinity;
        GameObject closestPlant = null;
        foreach (GameObject plant in plants)
        {
            PlantController plantController = plant.GetComponent<PlantController>();
            if (plantController != null && plantController.isGrown)
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
        }
        else
        {
            targetPlant = null;
            isCollecting = false;
        }
    }

    private void CollectPlant(PlantController plantController)
    {
        int grownPlantsCount = CountGrownPlants();
        if (grownPlantsCount >= maxPlantCarryCapacity)
        {
            plantValue += plantController.plants.value;
            carriedPlants.Add(plantController.plants);
            Destroy(plantController.gameObject);
            currentPlantCarryCount++;

            // Eğer taşıma kapasitesi doluysa satışa git
            if (currentPlantCarryCount >= maxPlantCarryCapacity)
            {
                isCollecting = false;
                isSelling = true;
            }
            else
            {
                // Aksi takdirde yeni bir bitki bul
                FindPlant();
            }
        }
        else
        {
            // Başlangıç konumuna dön
            agent.SetDestination(startLocation);
        }
    }

    // Olgun bitkilerin sayısını hesaplar
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
            carriedPlants.Clear();  // Listeyi temizle

            isSelling = false;

            FindPlant();
        }
    }
}