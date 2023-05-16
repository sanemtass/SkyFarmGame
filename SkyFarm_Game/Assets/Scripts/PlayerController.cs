using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float rotateSmooth;

    private Joystick joystick;
    private Rigidbody rb;
    private JoystickController joystickController;

    private void Awake()
    {
        joystick = FindObjectOfType<Joystick>();
        rb = GetComponent<Rigidbody>();
        joystickController = GetComponent<JoystickController>();
    }

    private void Start()
    {
        joystickController.SetJoystickParameters(joystick, transform, rb, moveSpeed, rotateSpeed, rotateSmooth);
    }

    private void FixedUpdate()
    {
        joystickController.GetJoystickController();
    }
}
