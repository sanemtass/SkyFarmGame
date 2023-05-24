using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedAreaController : MonoBehaviour
{
    // Eklenmiş bir bitki var mı diye kontrol etmek için bir bool değişkeni
    [SerializeField] private bool hasPlant = false;

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
        if((transform.childCount > 0))
        {
            hasPlant = true;
        }
        if ((transform.childCount < 0)) //yanlış
        {
            hasPlant = false;
        }

    }
}
