using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plants", menuName = "ScriptableObjects/Plants")]
public class Plants : ScriptableObject
{
    public enum PlantType
    {
        Plant1,
        Plant2,
        Plant3
    }

    public PlantType type;
    public string plantName;
    public GameObject plant;
    public GameObject newPlantPrefab; // Bitkinin toplandığında görünümü
    public float growthTime;
    public int value; // Öğenin fiyatı
}

