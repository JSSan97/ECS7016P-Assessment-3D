using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float scrollSpeed = 10;
    public float directionalSpeed = 10;
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
            // Move up
            move += Vector3.up * directionalSpeed;
        if (Input.GetKey(KeyCode.S))
            // Move down
            move -= Vector3.up * directionalSpeed;
        if (Input.GetKey(KeyCode.D))
            // Move left
            move += Vector3.right * directionalSpeed;
        if (Input.GetKey(KeyCode.A))
            // Move Right
            move -= Vector3.right * directionalSpeed;

        transform.Translate(move);

        // Zoom in and out using mouse wheel
        camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        // Use this if you don't have a mouse wheel
        // Zoom in
        if (Input.GetKey(KeyCode.Q))
            camera.fieldOfView -= scrollSpeed;
        // Zoom out
        if (Input.GetKey(KeyCode.E))
            camera.fieldOfView += scrollSpeed;
    }
}
