using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public Plants plants;
    public bool isGrown = false;

    private void Start()
    {
        // Büyüme sürecini başlat
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        // Bekleme süresi olarak bitkinin büyüme süresini kullan
        yield return new WaitForSeconds(plants.growthTime);

        // Bitki büyüdükten sonra yapılacak işlemler (örneğin, bitkinin görünümünü değiştirmek)

        Debug.Log("Plant has grown.");
        isGrown = true;

    }
}
