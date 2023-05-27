using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 400f;
    public float rotateSpeed;
    [Range(0,1)] [SerializeField] float rotateSmooth;

    private Joystick joystick;
    private Rigidbody rb;
    private JoystickController joystickController;
    private PlayerBehaviour playerBehaviour;

    private void Awake()
    {
        joystick = FindObjectOfType<Joystick>();
        rb = GetComponent<Rigidbody>();
        joystickController = GetComponent<JoystickController>();
    }

    private void Start()
    {
        playerBehaviour = GetComponent<PlayerBehaviour>();
        joystickController.SetJoystickParameters(joystick, transform, rb, playerBehaviour.animators, playerBehaviour.activeChildIndex, moveSpeed, rotateSpeed, rotateSmooth);
    }

    private void FixedUpdate()
    {
        joystickController.GetJoystickController();
    }
}
