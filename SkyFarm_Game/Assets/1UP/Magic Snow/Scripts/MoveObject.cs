using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveObject : MonoBehaviour
{
    public bool autoMove = false;
    [SerializeField]
    float moveSpeed = 2.0f;

    Vector3 centerPos = Vector3.zero;
    float angle = 0.0f;

    Vector3 moveDirection;
    float horizontal;
    float vertical;
    Rigidbody rb;
    bool shifted = false;
    bool jump = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        shifted |= Input.GetKeyDown(KeyCode.LeftShift);
        shifted |= Input.GetKeyDown(KeyCode.RightShift);
        shifted &= !Input.GetKeyUp(KeyCode.LeftShift);
        shifted &= !Input.GetKeyUp(KeyCode.RightShift);
        jump = Input.GetKeyDown(KeyCode.Space);
        Vector3 vOffset = vertical * Camera.main.transform.forward;
        Vector3 hOffset = horizontal * Camera.main.transform.right;

        moveDirection = Vector3.zero;
        if (!autoMove)
        {
            moveDirection = (vOffset + hOffset).normalized;
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.x = 1.5f * Mathf.Sin(angle);
            moveDirection.z = 1.5f * Mathf.Cos(angle);
            angle += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        float speedUp = shifted ? 2.0f : 0.0f;
        rb.velocity = (moveDirection * (moveSpeed + speedUp));
        if (moveDirection == Vector3.zero) return;
        rb.rotation = Quaternion.LookRotation(moveDirection);
    }
}
