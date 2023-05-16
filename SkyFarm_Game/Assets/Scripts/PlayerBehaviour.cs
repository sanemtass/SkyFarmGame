using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Stack<Plants> plantStack; // Bitkileri tutmak için bir Stack oluşturduk
    private float plantHeight = 0.5f; // Her bitkinin yüksekliği (bu değeri bitkinin gerçek yüksekliğine göre ayarlayabilirsiniz)
    public float distanceFromPlayer = 1.0f; // Player'dan bitkinin uzaklığı

    private void Start()
    {
        plantStack = new Stack<Plants>(); // Stack'ı başlat
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            // Bitki üzerindeki PlantController scriptine eriş
            PlantController plantController = other.GetComponent<PlantController>();

            // Eğer bitki üzerinde PlantController ve PlantBehaviour scripti var ve bitki büyümüşse
            if (plantController != null && plantController.isGrown == true)
            {
                plantStack.Push(plantController.plants); // Bitkiyi stack'e ekle

                // Bitkiyi player'ın bir child'ı yap
                other.transform.parent = transform;

                // Bitkiyi player'ın önünde ve bir önceki bitkinin üzerine yerleştir
                other.transform.localPosition = Vector3.forward * distanceFromPlayer + Vector3.up * (plantStack.Count - 1) * plantHeight;
                other.transform.localRotation = Quaternion.identity;
            }
        }
    }


    public void UsePlant()
    {
        if (plantStack.Count > 0)
        {
            Plants plant = plantStack.Pop(); // En son eklenen bitkiyi stack'den çıkar ve bitki objesini al
            Debug.Log(plant.plantName + " used."); // Bitki kullanıldı diye log'a yaz
        }
        else
        {
            Debug.Log("No plants in the stack!"); // Eğer stack boşsa, log'a yaz
        }
    }
}
