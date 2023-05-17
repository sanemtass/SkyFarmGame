using UnityEngine;

[CreateAssetMenu(fileName = "Variable", menuName = "Plus Umbrella/Variables/Create Float Variable", order = 0)]

public class FloatVariable : Variable<float>
{
    public void AddCount(int value = 1)
    {
        count += value;
    }
}