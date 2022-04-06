using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;

    private Vector3 eulerAngleVelocityClockwise = new Vector3(0, 180, 0);
    private Vector3 eulerAngleVelocityAntiClockwise = new Vector3(0, -180, 0);
    Rigidbody rigidbody;

    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W)) {
            rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * speed);
        }

        if(Input.GetKey(KeyCode.S)) {
            rigidbody.MovePosition(transform.position - transform.right * Time.fixedDeltaTime * speed);
        }

        Quaternion deltaRotationClockwise = Quaternion.Euler(eulerAngleVelocityClockwise * Time.fixedDeltaTime);
        Quaternion deltaRotationAntiClockwise = Quaternion.Euler(eulerAngleVelocityAntiClockwise * Time.fixedDeltaTime);

        if(Input.GetKey(KeyCode.D))
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotationClockwise);

        if(Input.GetKey(KeyCode.A))
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotationAntiClockwise);
    }
}
