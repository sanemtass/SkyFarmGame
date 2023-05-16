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

    public void SetJoystickParameters(Joystick joystick, Transform transform, Rigidbody rigidbody, float moveSpeed, float rotateSpeed, float rotateSmooth)
    {
        this.joystick = joystick;
        this.colletorTransform = transform;
        this.collectorRigidbody = rigidbody;
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
            Vector3 collectPos = new Vector3(horizontal, 0, vertical);
            collectorRigidbody.AddForce(collectPos * collectorMoveSpeed * Time.deltaTime);//Move with physics.
                                                                                          //Vector3 movePos = new Vector3(horizontal * moveSpeed * Time.deltaTime, 0, vertical * moveSpeed * Time.deltaTime);//Move with transform. 
                                                                                          // transform.position += movePos;

            Vector3 direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
            colletorTransform.rotation = Quaternion.Slerp(colletorTransform.rotation, Quaternion.LookRotation(direction), collectorRotateSmooth);
        }

        else
        {
            collectorRigidbody.velocity = Vector3.zero;
        }
    }
}
