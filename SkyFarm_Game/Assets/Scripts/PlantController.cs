using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlantController : MonoBehaviour
{
    public Plants plants;
    public bool isGrown = false;
    public bool isCollected = false;

    private void Start()
    {
        StartCoroutine(Grow());
        isGrown = true;
    }

    private IEnumerator Grow()
    {
        yield return new WaitForSeconds(plants.growthTime);

        Debug.Log("Plant has grown.");
    }
}
