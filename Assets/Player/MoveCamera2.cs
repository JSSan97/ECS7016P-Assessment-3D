using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera2 : MonoBehaviour
{
    public GameObject camera;
    private Vector3 offset; 
    private float scrollSpeed = 1.0f;

    void Update()
    {
        camera.transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z - 5.0f);

        // Zoom in and out using mouse wheel
        camera.GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
    }
}
