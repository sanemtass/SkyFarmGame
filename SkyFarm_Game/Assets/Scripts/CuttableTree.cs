using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableTree : MonoBehaviour
{
    public bool isCut = false;
    public GameObject topPart; // Küpün üst kısmını temsil eden bir nesne
    public GameObject bottomPart; // Küpün alt kısmını temsil eden bir nesne

    private void Start()
    {
        // Başlangıçta, küp tam ve kesilmemiş durumdadır
        topPart.SetActive(false);
        bottomPart.SetActive(true);
    }

    public void Cut()
    {
        // Küpü keser ve üst ve alt parçaları aktive eder
        isCut = true;
        topPart.SetActive(true);
        bottomPart.SetActive(true);

        // Üst kısma Rigidbody bileşeni ekle ve yerçekimini etkinleştir
        Rigidbody rb = topPart.AddComponent<Rigidbody>();
        rb.useGravity = true;
    }
}
