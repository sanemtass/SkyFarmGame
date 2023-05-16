using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArea", menuName = "ScriptableObjects/PlantedArea")]
public class PlantedArea : ScriptableObject
{
    public string itemName;
    public int price; // Öğenin fiyatı
    // Diğer özellikler
}
