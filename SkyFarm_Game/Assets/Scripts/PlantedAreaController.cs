using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedAreaController : MonoBehaviour
{
    // Eklenmiş bir bitki var mı diye kontrol etmek için bir bool değişkeni
    [SerializeField] private bool hasPlant = false;
    [SerializeField] private PlayerBehaviour playerBehaviour;

    public AudioClip plantSound; // Bitki eklenince çalacak ses dosyası (Unity editöründen ayarlanacak)
    private AudioSource audioSource;
    private bool isGrowing = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSource bileşenini alın (bu bileşenin objeye ekli olduğundan emin olun)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGrowing)
        {
            // Eğer zaten bir bitki ekilmişse, daha fazla bitki eklemeyi engelle
            if (hasPlant)
            {
                Debug.Log("A plant has already been planted here.");
                return;
            }

            Plants selectedPlant = GameManager.Instance.GetSelectedPlant();

            StartCoroutine(GrowthDelay(selectedPlant));
        }

        if (other.CompareTag("Player"))
        {
            PlayerBehaviour playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                playerBehaviour.PlayPlantingAnimation();
            }
        }
    }

    private IEnumerator GrowthDelay(Plants selectedPlant)
    {
        isGrowing = true;

        yield return new WaitForSeconds(selectedPlant.growthTime);

        // Seçili bitkinin Plants ScriptableObject'ını al
        for (int i = 0; i < 1; i++)
        {
            if (selectedPlant != null)
            {
                // Seçili bitkinin prefab'ını al ve bu konumda oluştur
                GameObject plantPrefab = selectedPlant.plant;
                GameObject newPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
                Debug.Log("NEDEN");

                // Oluşturulan bitkiyi PlantedAreaController'ın alt nesnesi olarak ayarla
                newPlant.transform.SetParent(transform);

                // Bitki ekildiğini belirt
                hasPlant = true;

                if (audioSource != null && plantSound != null)
                {
                    audioSource.PlayOneShot(plantSound);
                }
            }
        }
        
        isGrowing = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // Bitkinin durumunu güncelle
        int plantCount = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Plant")
            {
                plantCount++;
            }
        }

        if (plantCount > 0)
        {
            hasPlant = true;
        }

        else if (plantCount == 0)
        {
            hasPlant = false;
        }
    }
}
