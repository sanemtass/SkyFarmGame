using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour
{
    private Stack<Plants> plantStack;
    private float plantHeight = 1f;
    public float distanceFromPlayer = 1.0f;
    public Transform salesArea;
    public GameObject handObject;
    public GameObject[] childObjectsWithAnimators;
    public Animator[] animators;
    public int activeChildIndex = 0;
    public GameObject[] handObjects;
    public int activeHandIndex = 0;
    public float speed = 400f;
    public int maxPlantCapacity = 3;
    public GameObject ax, tree;
    private bool isStartingAnimationFinished = false;

    public AudioClip pickUpSound, salesSound, childSwitchSound;
    private AudioSource audioSource;

    private PlayerController playerController;

    public ParticleSystem coinParticle, levelUpParticle;

    [SerializeField] private Stack<GameObject> plantObjects;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animators = new Animator[childObjectsWithAnimators.Length];
        for (int i = 0; i < childObjectsWithAnimators.Length; i++)
        {
            animators[i] = childObjectsWithAnimators[i].GetComponent<Animator>();
        }

        if (handObjects.Length > 0)
        {
            for (int i = 0; i < handObjects.Length; i++)
            {
                if (i == activeHandIndex)
                {
                    handObjects[i].SetActive(true);
                    handObject = handObjects[i];
                }
                else
                {
                    handObjects[i].SetActive(false); 
                }
            }
        }

        SwitchChildObject(activeChildIndex);
    }

    private void Start()
    {
        plantStack = new Stack<Plants>();
        plantObjects = new Stack<GameObject>(); 
        playerController = GetComponent<PlayerController>();

        PlayAnimation("Hand");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CamFollow camFollow = Camera.main.GetComponent<CamFollow>();
            camFollow.StartFollowing();

            // Idle animasyonuna dön
            if (animators[activeChildIndex] != null)
            {
                animators[activeChildIndex].Play("Idle");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            PlantController plantController = other.GetComponent<PlantController>();

            if (plantController != null && plantController.isGrown == true && plantController.isCollected == false)
            {
                if (plantStack.Count >= maxPlantCapacity)
                {
                    Debug.Log("Player can't carry any more plants!");
                    return;
                }

                plantStack.Push(plantController.plants);

                Destroy(other.gameObject);

                GameObject newPlant = Instantiate(plantController.plants.newPlantPrefab, other.transform.position, Quaternion.identity, handObject.transform);

                newPlant.transform.localPosition = Vector3.forward * distanceFromPlayer + Vector3.up * (plantStack.Count - 1) * plantHeight;
                plantObjects.Push(newPlant);

                plantController.isCollected = true;

                if (audioSource != null && pickUpSound != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }

                UpdateAnimatorState();
            }
        }

        if (other.CompareTag("SalesArea"))
        {
            UsePlant();

            Debug.Log("GORDUN MU");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            foreach (Animator animator in animators)
            {
                if (animator.gameObject.activeInHierarchy)
                {
                    animator.Play("CutTree");
                    ax.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlantedArea"))
        {
            if (animators[activeChildIndex] != null)
            {
                animators[activeChildIndex].SetBool("IsPlanting", false);
            }
        }

        if (other.CompareTag("Tree"))
        {
            foreach (Animator animator in animators)
            {
                if (animator.gameObject.activeInHierarchy)
                {
                    animator.Play("Idle");
                    
                }
            }
            Invoke(nameof(TreeFalse), 2f);
        }
    }

    private void TreeFalse()
    {
        tree.SetActive(false);
        ax.SetActive(false);
    }

    public void UsePlant()
    {
        if (plantObjects.Count > 0)
        {
            GameObject plantObject = plantObjects.Pop(); // En son eklenen bitkinin GameObject versiyonunu stack'den çıkar
            Plants plant = plantStack.Pop(); // En son eklenen bitkiyi stack'den çıkar
            GameManager.Instance.ChangeGold(plant.value);
            MoveToSalesArea(plantObject);
        }
        else
        {
            Debug.Log("No plants in the stack!");
        }

        UpdateAnimatorState();
    }

    private void MoveToSalesArea(GameObject plantObject)
    {
        Vector3 endPosition = salesArea.transform.position;
        float journeyTime = 1.0f;
        float jumpPower = 2f;
        int numJumps = 1;

        plantObject.transform.DOLookAt(salesArea.transform.position, journeyTime);
        plantObject.transform.DOJump(endPosition, jumpPower, numJumps, journeyTime)
            .OnComplete(() =>
            {
                Destroy(plantObject);
                UpdateAnimatorState();
                if (audioSource != null && pickUpSound != null)
                {
                    audioSource.PlayOneShot(salesSound);
                }
                coinParticle.Play();
            });
    }

    private void UpdateAnimatorState()
    {
        if (animators[activeChildIndex] != null)
        {
            Debug.Log("ANimasyon calisio mu");
            animators[activeChildIndex].SetInteger("PlantCount", plantObjects.Count);
            if (plantObjects.Count > 0)
            {
                animators[activeChildIndex].SetBool("IsPlantStacking", true);
            }
            else
            {
                animators[activeChildIndex].SetBool("IsPlantStacking", false);
                animators[activeChildIndex].SetBool("IsWalking", false);
            }
        }
    }

    public void SwitchChildObject(int newChildIndex)
    {
        if (newChildIndex >= 0 && newChildIndex < childObjectsWithAnimators.Length)
        {
            if (childObjectsWithAnimators[newChildIndex] == null)
            {
                Debug.LogError("Child object is null at index " + newChildIndex);
                return;
            }

            Animator animator = childObjectsWithAnimators[newChildIndex].GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator component on child object at index " + newChildIndex);
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("No AnimatorController set on Animator of child object at index " + newChildIndex);
                return;
            }

            childObjectsWithAnimators[activeChildIndex].SetActive(false);
            childObjectsWithAnimators[newChildIndex].SetActive(true);
            animator.Rebind(); //resetle ve başlat
            activeChildIndex = newChildIndex;

            if (audioSource != null && childSwitchSound != null)
            {
                audioSource.PlayOneShot(childSwitchSound);
            }

            levelUpParticle.Play();

            SwitchHandObject(activeChildIndex);
        }
    }

    public void SwitchHandObject(int newHandIndex)
    {
        if (newHandIndex >= 0 && newHandIndex < handObjects.Length && newHandIndex != activeHandIndex)
        {
            handObjects[activeHandIndex].SetActive(false);

            handObjects[newHandIndex].SetActive(true);

            handObject = handObjects[newHandIndex];
            activeHandIndex = newHandIndex;
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].Play(animationName);
        }
    }

    public void PlayPlantingAnimation()
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].SetBool("IsPlanting", true);
            Invoke(nameof(IdleToPlanting), 1.5f);
        }
    }

    private void IdleToPlanting()
    {
        if (animators[activeChildIndex] != null)
        {
            animators[activeChildIndex].SetBool("IsPlanting", false);
            animators[activeChildIndex].Play("Idle");
        }
    }

    public void UpgradeCharacter(int newIndex)
    {
        if (newIndex == 1)
        {
            speed += 100;
            playerController.moveSpeed = speed;
            maxPlantCapacity += 2;
        }
        else if (newIndex == 2)
        {
            speed += 200;
            playerController.moveSpeed = speed;
            maxPlantCapacity += 3;
        }
    }

}