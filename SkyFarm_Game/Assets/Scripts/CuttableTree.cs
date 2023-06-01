using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableTree : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator playerAnimator = other.GetComponent<Animator>();

            if (playerAnimator != null)
            {
                playerAnimator.Play("CutTree");
            }
        }
    }
}
