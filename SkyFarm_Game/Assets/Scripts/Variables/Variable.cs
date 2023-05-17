using UnityEngine;

[CreateAssetMenu(fileName = "Variable", menuName = "Plus Umbrella/Variables/Create Variable", order = 0)]
public class Variable<T> : ScriptableObject
{
    public T count;
}
