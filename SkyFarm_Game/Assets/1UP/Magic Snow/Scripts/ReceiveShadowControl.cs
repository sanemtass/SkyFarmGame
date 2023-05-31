using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveShadowControl : MonoBehaviour
{
    bool receiveShadow = false;
    Renderer renderer = null;
    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponent<MeshRenderer>();
        if (renderer == null) return;
        receiveShadow = renderer.receiveShadows;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            receiveShadow = !receiveShadow;
        }
    }

    private void FixedUpdate()
    {
        if (renderer == null) return;
        renderer.receiveShadows = receiveShadow;
    }
}
