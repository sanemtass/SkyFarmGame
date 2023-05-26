using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedAreaController : MonoBehaviour
{
    // Eklenmiş bir bitki var mı diye kontrol etmek için bir bool değişkeni
    [SerializeField] private bool hasPlant = false;
    [SerializeField] private PlayerBehaviour playerBehaviour;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Eğer zaten bir bitki ekilmişse, daha fazla bitki eklemeyi engelle
            if (hasPlant)
            {
                Debug.Log("A plant has already been planted here.");
                return;
            }

            // Seçili bitkinin Plants ScriptableObject'ını al
            Plants selectedPlant = GameManager.Instance.GetSelectedPlant();

            if (selectedPlant != null)
            {
                // Seçili bitkinin prefab'ını al ve bu konumda oluştur
                GameObject plantPrefab = selectedPlant.plant;
                GameObject newPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);

                // Oluşturulan bitkiyi PlantedAreaController'ın alt nesnesi olarak ayarla
                newPlant.transform.SetParent(transform);

                // Bitki ekildiğini belirt
                hasPlant = true;
            }
        }
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

        if (other.CompareTag("Player"))
        {
            PlayerBehaviour playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                playerBehaviour.PlayPlantingAnimation();
            }
        }
    }
}
