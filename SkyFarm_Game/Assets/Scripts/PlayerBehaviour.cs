using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour
{
    private Stack<Plants> plantStack; // Bitkileri tutmak için bir Stack oluşturduk
    private float plantHeight = 0.5f; // Her bitkinin yüksekliği (bu değeri bitkinin gerçek yüksekliğine göre ayarlayabilirsiniz)
    public float distanceFromPlayer = 1.0f; // Player'dan bitkinin uzaklığı
    public Transform salesArea;

    private Stack<GameObject> plantObjects; // Bitkilerin GameObject versiyonlarını tutmak için bir Stack oluşturduk

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

            // Eğer bitki üzerinde PlantController ve PlantBehaviour scripti var ve bitki büyümüşse
            if (plantController != null && plantController.isGrown == true)
            {
                plantStack.Push(plantController.plants); // Bitkiyi stack'e ekle
                plantObjects.Push(other.gameObject); // Bitkinin GameObject versiyonunu stack'e ekle

                // Bitkiyi player'ın bir child'ı yap
                other.transform.parent = transform;

                // Bitkiyi player'ın önünde ve bir önceki bitkinin üzerine yerleştir
                other.transform.localPosition = Vector3.forward * distanceFromPlayer + Vector3.up * (plantStack.Count - 1) * plantHeight;
                other.transform.localRotation = Quaternion.identity;
            }
        }

        if (other.CompareTag("SalesArea"))
        {
            UsePlant();
            Debug.Log("GORDUN MU");
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
    }

    private void MoveToSalesArea(GameObject plantObject)
    {
        Vector3 endPosition = salesArea.transform.position; // Bitkinin hedef konumu (Sales Area'nın konumu)
        float journeyTime = 1.0f; // Bitkinin başlangıç konumundan hedef konuma gitmesi için gereken süre
        float jumpPower = 2f; // Bitkinin zıplama gücü
        int numJumps = 1; // Bitkinin kaç kere zıplayacağı

        plantObject.transform.DOJump(endPosition, jumpPower, numJumps, journeyTime)
        .OnComplete(() => Destroy(plantObject));
    }
}
