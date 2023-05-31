using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PenquinAnim : MonoBehaviour
{
    private Animator animator;

    bool shifted = false;
    bool prevShifted = false;
    bool jump = false;
    bool move = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        move = false;
        move = 
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);
        shifted = Input.GetKey(KeyCode.LeftShift);
        shifted |= Input.GetKey(KeyCode.RightShift);
        jump = Input.GetKeyDown(KeyCode.Space);
        if (animator == null) return;
        animator.SetBool("Walk", move);
        if (prevShifted != shifted)
        {
            animator.SetBool("Run", shifted);
            prevShifted = shifted;
        }
        animator.SetBool("Jump", jump);
    }

    private void FixedUpdate()
    {
    }
}
