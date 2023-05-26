using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour
{
    private Stack<Plants> plantStack; // Bitkileri tutmak için bir Stack oluşturduk
    private float plantHeight = 1f; // Her bitkinin yüksekliği (bu değeri bitkinin gerçek yüksekliğine göre ayarlayabilirsiniz)
    public float distanceFromPlayer = 1.0f; // Player'dan bitkinin uzaklığı
    public Transform salesArea;
    public GameObject handObject; // Hand objesine bir referans ekleyin
    public GameObject[] childObjectsWithAnimators; // Unity inspector'da ayarlayın
    private Animator[] animators;
    private int activeChildIndex = 0;


    [SerializeField] private Stack<GameObject> plantObjects; // Bitkilerin GameObject versiyonlarını tutmak için bir Stack oluşturduk

    private void Awake()
    {
        animators = new Animator[childObjectsWithAnimators.Length];
        for (int i = 0; i < childObjectsWithAnimators.Length; i++)
        {
            animators[i] = childObjectsWithAnimators[i].GetComponent<Animator>();
        }
    }

    private void Start()
    {
        plantStack = new Stack<Plants>(); // Plants Stack'ı başlat
        plantObjects = new Stack<GameObject>(); // GameObject Stack'ı başlat
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            // Bitki üzerindeki PlantController scriptine eriş
            PlantController plantController = other.GetComponent<PlantController>();

            // Eğer bitki üzerinde PlantController ve PlantBehaviour scripti var, bitki büyümüş ve henüz toplanmamışsa
            if (plantController != null && plantController.isGrown == true && plantController.isCollected == false)
            {
                // Eğer stack'in boyutu zaten 3 veya daha büyükse, yeni bitki eklemeyi engelle
                if (plantStack.Count >= 3)
                {
                    Debug.Log("Player can't carry any more plants!");
                    return;
                }

                plantStack.Push(plantController.plants);

                Destroy(other.gameObject); // Eski bitkiyi yok et

                // Yeni bitkiyi oluştur ve stack'e ekle
                GameObject newPlant = Instantiate(plantController.plants.newPlantPrefab, other.transform.position, Quaternion.identity, handObject.transform);

                newPlant.transform.localPosition = Vector3.forward * distanceFromPlayer + Vector3.up * (plantStack.Count - 1) * plantHeight;
                plantObjects.Push(newPlant);

                plantController.isCollected = true;
                UpdateAnimatorState();
            }
        }

        if (other.CompareTag("Cube"))
        {
            CuttableTree cuttableTree = other.GetComponent<CuttableTree>();
            Debug.Log("A");

            if (cuttableTree != null && !cuttableTree.isCut)
            {
                Debug.Log("B");
                cuttableTree.Cut();
            }
        }

        if (other.CompareTag("SalesArea"))
        {
            UsePlant();
            Debug.Log("GORDUN MU");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlantedArea"))
        {
            if (animators[activeChildIndex] != null)
            {
                animators[activeChildIndex].SetBool("IsPlanting", false);
            }
        }
    }

    public void UsePlant()
    {
        if (plantObjects.Count > 0)
        {
            GameObject plantObject = plantObjects.Pop(); // En son eklenen bitkinin GameObject versiyonunu stack'den çıkar
            Plants plant = plantStack.Pop(); // En son eklenen bitkiyi stack'den çıkar
            GameManager.Instance.ChangeGold(plant.value); // Altını arttır ve olayı tetikle
            MoveToSalesArea(plantObject); // Bitkiyi Sales Area'ya yolla
        }
        else
        {
            Debug.Log("No plants in the stack!"); // Eğer stack boşsa, log'a yaz
        }
        UpdateAnimatorState();
    }

    private void MoveToSalesArea(GameObject plantObject)
    {
        Vector3 endPosition = salesArea.transform.position; // Bitkinin hedef konumu (Sales Area'nın konumu)
        float journeyTime = 1.0f; // Bitkinin başlangıç konumundan hedef konuma gitmesi için gereken süre
        float jumpPower = 2f; // Bitkinin zıplama gücü
        int numJumps = 1; // Bitkinin kaç kere zıplayacağı

        plantObject.transform.DOJump(endPosition, jumpPower, numJumps, journeyTime)
        .OnComplete(() =>
        {
            Destroy(plantObject);
            UpdateAnimatorState();
        });
    }

    private void UpdateAnimatorState()
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].SetInteger("PlantCount", plantObjects.Count);
            if (plantObjects.Count > 0)
            {
                animators[activeChildIndex].SetBool("IsPlantStacking", true);
            }
            else
            {
                animators[activeChildIndex].SetBool("IsPlantStacking", false);
                animators[activeChildIndex].SetBool("IsWalking", false);
            }
        }
    }

    public void SwitchChildObject(int newChildIndex)
    {
        if (newChildIndex >= 0 && newChildIndex < childObjectsWithAnimators.Length)
        {
            animators[activeChildIndex].SetBool("IsPlantStacking", false); // Eski çocuğun animasyonlarını sıfırla
            animators[activeChildIndex].SetBool("IsWalking", false); // Eski çocuğun animasyonlarını sıfırla

            childObjectsWithAnimators[activeChildIndex].SetActive(false); // Eski çocuğu kapat
            childObjectsWithAnimators[newChildIndex].SetActive(true); // Yeni çocuğu aç
            activeChildIndex = newChildIndex; // Aktif çocuğun indexini güncelle
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].Play(animationName);
        }
    }

    public void PlayPlantingAnimation()
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].SetBool("IsPlanting", true);
        }
    }
}