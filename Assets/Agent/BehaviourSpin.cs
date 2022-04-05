using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class BehaviourSpin : CustomBehaviour
{
    private Rigidbody rigidbody;  
    private SteeringBasics steeringBasics;
    // Rotate around the y axis 90 degrees/sec
    private Vector3 eulerAngleVelocity = new Vector3(0, 90, 0);
    public BehaviourSpin(SteeringBasics steeringBasics, Rigidbody rigidbody){
        this.rigidbody = rigidbody;
        this.steeringBasics = steeringBasics;
    }

    public override void Perform()
    {
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
        rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        steeringBasics.LookWhereYoureGoing();
        // float y = rigidbody.Rotation.y;
        // rigidbody.MoveRotation(y + (90 * Time.fixedDeltaTime));
    }
}
