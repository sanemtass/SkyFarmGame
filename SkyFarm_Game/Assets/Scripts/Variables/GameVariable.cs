using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Variable", menuName = "Plus Umbrella/Variables/Create Game Variable", order = 0)]

public class GameVariable : IntVariable
{
    public int level = 1;
    public int cost = 200;
    public int maxLevel;
    public float increaseCost = 1.3f;

    public void LevelUp()
    {
        level++;
        cost = (int)(cost * increaseCost);
        AddCount();
    }
}
