using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject LevelOneLand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(".");
            GameManager.Instance.OnNewLandFullyActive();
            LevelOneLand.SetActive(false);
        }
    }
}
