using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 2f;
    //public bool isRotate;
    //public Vector3 quaternionPos;

    private float distanceTravelled;

    private void Start()
    {
        //if (isRotate)
        //{
        //    transform.rotation = Quaternion.Euler(quaternionPos);
        //}
    }

    private void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }
}
