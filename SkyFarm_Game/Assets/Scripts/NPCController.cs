using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject targetPlant; // Hedef bitki
    private bool isCollecting = false;
    private bool isSelling = false; // NPC'nin bitki satmak için SalesArea'ya gidiyor olup olmadığını kontrol eder

    public Transform salesArea; // SalesArea'nın Transform componenti

    [SerializeField] private int maxPlantCarryCapacity; // NPC'nin taşıyabileceği maksimum bitki sayısı
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

        // Taşıma kapasitesini 2 ile 5 arasında bir sayı olarak belirle
        maxPlantCarryCapacity = Random.Range(2, 5); // Random.Range üst limiti dahil etmediği için 6 yazıyoruz
        Debug.Log(maxPlantCarryCapacity);
    }

    private void Update()
    {
        if (isSelling) // Eğer NPC satış yapmak için SalesArea'ya doğru hareket ediyorsa
        {
            // SalesArea'ya doğru hareket et
            agent.SetDestination(salesArea.position);

            // SalesArea'ya yeterince yaklaştıysa
            if (Vector3.Distance(transform.position, salesArea.position) < agent.stoppingDistance)
            {
                // Bitkiyi sat ve altın miktarını arttır
                SellPlant();
            }
        }
        else if (!isCollecting) // Eğer bir bitki toplanmak üzere işaretlenmemişse, yeni bir bitki ara
        {
            FindPlant();
        }
        else if (targetPlant != null && isCollecting) // Eğer hedeflenen bitki varsa ve bitki toplanmak üzere işaretlenmişse
        {
            // Hedeflenen bitkiye doğru hareket et
            agent.SetDestination(targetPlant.transform.position);
        }
        else
        {
            isCollecting = false; // Bitki artık hedeflenmiyor, yeni bir bitki ara
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
        // Bitkileri ara ve en yakın olanı hedefle
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

        // Yeni bir hedef bitki belirle
        if (closestPlant != null)
        {
            targetPlant = closestPlant;
            isCollecting = true;
        }
        else
        {
            targetPlant = null; // Hedef bitki yok
            isCollecting = false;
        }
    }

    private void CollectPlant(PlantController plantController)
    {
        // Toplanan bitkilerin değerini sakla
        plantValue += plantController.plants.value;

        // Bitkiyi yok et
        Destroy(plantController.gameObject);

        // NPC'nin taşıdığı bitki sayısını artır
        currentPlantCarryCount++;

        // Eğer NPC taşıma kapasitesine ulaştıysa
        if (currentPlantCarryCount >= maxPlantCarryCapacity)
        {
            // Bitki toplamayı durdur
            isCollecting = false;

            // Geriye kalan bitkilerin sayısı taşıma kapasitesinden fazlaysa,
            // NPC'nin başlangıç konumuna dönecek şekilde ayarla
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

            if (grownPlantsCount > maxPlantCarryCapacity)
            {
                agent.SetDestination(startLocation); // Başlangıç konumuna geri dön
            }
            else
            {
                isSelling = true; // Satış alanına git
            }
        }
        else
        {
            // Aksi takdirde yeni bir bitki bul
            FindPlant();
        }
    }

    private void SellPlant()
    {
        if (isSelling)
        {
            // Bitkinin değerini toplam altına ekleyin
            GameManager.Instance.gold.count += plantValue;

            // Altın değişikliğini yayınla
            GameManager.Instance.ChangeGold(plantValue);

            isSelling = false;

            // Bitkileri sattıktan sonra bitki taşıma sayısını ve bitki değerini sıfırla
            currentPlantCarryCount = 0;
            plantValue = 0;

            // Satış yaptıktan sonra yeni bir bitki ara
            FindPlant();
        }
    }

}