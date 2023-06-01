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
    public Animator[] animators;
    public int activeChildIndex = 0;
    public GameObject[] handObjects; // Elin objelerine bir dizi referansı ekleyin
    public int activeHandIndex = 0;
    public float speed = 400f;
    public int maxPlantCapacity = 3;

    private PlayerController playerController;

    [SerializeField] private Stack<GameObject> plantObjects; // Bitkilerin GameObject versiyonlarını tutmak için bir Stack oluşturduk

    private void Awake()
    {
        animators = new Animator[childObjectsWithAnimators.Length];
        for (int i = 0; i < childObjectsWithAnimators.Length; i++)
        {
            animators[i] = childObjectsWithAnimators[i].GetComponent<Animator>();
        }

        if (handObjects.Length > 0)
        {
            for (int i = 0; i < handObjects.Length; i++)
            {
                if (i == activeHandIndex)
                {
                    handObjects[i].SetActive(true);  // Aktif el objesini açın
                    handObject = handObjects[i];
                }
                else
                {
                    handObjects[i].SetActive(false);  // Diğer el objelerini kapatın
                }
            }
        }

        SwitchChildObject(activeChildIndex);
    }

    private void Start()
    {
        plantStack = new Stack<Plants>(); // Plants Stack'ı başlat
        plantObjects = new Stack<GameObject>(); // GameObject Stack'ı başlat
        playerController = GetComponent<PlayerController>();
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
                // Eğer stack'in boyutu zaten maxPlantCapacity veya daha büyükse, yeni bitki eklemeyi engelle
                if (plantStack.Count >= maxPlantCapacity)
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

        if (other.CompareTag("SalesArea"))
        {
            UsePlant();
            Debug.Log("GORDUN MU");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            foreach (Animator animator in animators)
            {
                if (animator.gameObject.activeInHierarchy)
                {
                    animator.Play("CutTree");
                }
            }
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

        if (other.CompareTag("Tree"))
        {
            foreach (Animator animator in animators)
            {
                if (animator.gameObject.activeInHierarchy)
                {
                    animator.Play("Idle"); // Burada "Idle" animasyon adınız olmalı
                }
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
            if (childObjectsWithAnimators[newChildIndex] == null)
            {
                Debug.LogError("Child object is null at index " + newChildIndex);
                return;
            }

            Animator animator = childObjectsWithAnimators[newChildIndex].GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator component on child object at index " + newChildIndex);
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("No AnimatorController set on Animator of child object at index " + newChildIndex);
                return;
            }

            childObjectsWithAnimators[activeChildIndex].SetActive(false); // Eski çocuğu kapat
            childObjectsWithAnimators[newChildIndex].SetActive(true); // Yeni çocuğu aç
            animator.Rebind(); // Yeni çocuğun animatörünü resetle ve başlat
            activeChildIndex = newChildIndex; // Aktif çocuğun indexini güncelle

            SwitchHandObject(activeChildIndex); // Elin indexini güncelle
        }
    }

    public void SwitchHandObject(int newHandIndex)
    {
        // Eğer yeni dizin geçerli bir dizinse ve farklı bir dizinse
        if (newHandIndex >= 0 && newHandIndex < handObjects.Length && newHandIndex != activeHandIndex)
        {
            // Eski aktif eli deaktif hale getir
            handObjects[activeHandIndex].SetActive(false);

            // Yeni aktif eli aktif hale getir
            handObjects[newHandIndex].SetActive(true);

            // Aktif eli değiştir
            handObject = handObjects[newHandIndex];
            activeHandIndex = newHandIndex;
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

    public void UpgradeCharacter(int newIndex)
    {
        // Karakterin taşıma kapasitesini ve hızını yükseltin
        if (newIndex == 1)
        {
            speed += 100;
            playerController.moveSpeed = speed;  // Karakterin hızını güncelle
            maxPlantCapacity += 2;
        }
        else if (newIndex == 2)
        {
            speed += 200;  // Örneğin, hızı daha fazla artırabiliriz
            playerController.moveSpeed = speed;  // Karakterin hızını güncelle
            maxPlantCapacity += 3;  // Örneğin, taşıma kapasitesini daha fazla artırabiliriz
        }
    }

}