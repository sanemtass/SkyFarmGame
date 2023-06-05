using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedAreaController : MonoBehaviour
{
    [SerializeField] private bool hasPlant = false;
    [SerializeField] private PlayerBehaviour playerBehaviour;

    public AudioClip plantSound;
    private AudioSource audioSource;
    private bool isGrowing = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGrowing)
        {
            if (hasPlant)
            {
                Debug.Log("A plant has already been planted here.");
                return;
            }

            Plants selectedPlant = GameManager.Instance.GetSelectedPlant();

            StartCoroutine(GrowthDelay(selectedPlant));
        }

        if (other.CompareTag("Player"))
        {
            PlayerBehaviour playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                playerBehaviour.PlayPlantingAnimation();
            }
        }
    }

    private IEnumerator GrowthDelay(Plants selectedPlant)
    {
        isGrowing = true;

        yield return new WaitForSeconds(selectedPlant.growthTime);

        for (int i = 0; i < 1; i++)
        {
            if (selectedPlant != null)
            {
                GameObject plantPrefab = selectedPlant.plant;
                GameObject newPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);

                newPlant.transform.SetParent(transform);

                hasPlant = true;

                if (audioSource != null && plantSound != null)
                {
                    audioSource.PlayOneShot(plantSound);
                }
            }
        }
        
        isGrowing = false;
    }

    private void OnTriggerStay(Collider other)
    {
        int plantCount = 0;

        foreach (Transform child in transform)
        {
            if (child.tag == "Plant")
            {
                plantCount++;
            }
        }

        if (plantCount > 0)
        {
            hasPlant = true;
        }

        else if (plantCount == 0)
        {
            hasPlant = false;
        }
    }
}
