using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    private Joystick joystick;
    private Transform colletorTransform;
    private Rigidbody collectorRigidbody;
    private float collectorMoveSpeed;
    private float collectorRotateSpeed;
    private float collectorRotateSmooth;

    public bool IsWalking { get; private set; } = false;
    public Animator[] animators;
    public int activeChildIndex = 0;

    public void SetJoystickParameters(Joystick joystick, Transform transform, Rigidbody rigidbody, Animator[] animators, int activeChildIndex, float moveSpeed, float rotateSpeed, float rotateSmooth)
    {
        this.joystick = joystick;
        this.colletorTransform = transform;
        this.collectorRigidbody = rigidbody;
        this.animators = animators;
        this.activeChildIndex = activeChildIndex;
        this.collectorMoveSpeed = moveSpeed;
        this.collectorRotateSpeed = rotateSpeed;
        this.collectorRotateSmooth = rotateSmooth;
    }

    public void GetJoystickController()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        if (joystick.Horizontal != 0 && joystick.Vertical != 0)
        {
            IsWalking = true;
            Vector3 collectPos = new Vector3(horizontal, 0, vertical);
            collectorRigidbody.AddForce(collectPos * collectorMoveSpeed * Time.deltaTime);

            animators[activeChildIndex].SetBool("IsWalking", IsWalking);

            Vector3 direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
            colletorTransform.rotation = Quaternion.Slerp(colletorTransform.rotation, Quaternion.LookRotation(direction), collectorRotateSmooth);
            collectorRigidbody.isKinematic = false;
        }
        else
        {
            IsWalking = false;
            animators[activeChildIndex].SetBool("IsWalking", IsWalking);
            animators[activeChildIndex].SetBool("IsPlantStacking", false);
            collectorRigidbody.velocity = Vector3.zero;
            collectorRigidbody.isKinematic = true;
        }
    }

    public void SwitchChildObject(int newIndex)
    {
        // Eğer yeni index aktif olanla aynıysa değişiklik yapma
        if (newIndex == activeChildIndex)
        {
            return;
        }

        // Eski çocuk nesnenin animatörünü kapalı hale getir
        animators[activeChildIndex].enabled = false;

        // Yeni çocuk nesnenin animatörünü açık hale getir
        animators[newIndex].enabled = true;

        // Aktif çocuk nesnenin indexini güncelle
        activeChildIndex = newIndex;
    }
}
