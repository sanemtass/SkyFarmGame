using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public bool isFollowing = false;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void ResetCameraPosition()
    {
        transform.position = originalPosition;
        isFollowing = false;
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }

    private void FixedUpdate()
    {
        if (target != null && isFollowing)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
