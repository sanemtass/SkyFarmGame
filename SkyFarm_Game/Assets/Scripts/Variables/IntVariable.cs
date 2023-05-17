using UnityEngine;

[CreateAssetMenu(fileName = "Variable", menuName = "Plus Umbrella/Variables/Create Int Variable", order = 0)]
public class IntVariable : Variable<int>
{
    public void AddCount(int value = 1)
    {
        count += value;
    }
}
