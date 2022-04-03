using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class CustomSeek : CustomBehaviour
{
    public Transform target;

    SteeringBasics steeringBasics;

    public CustomSeek(SteeringBasics steeringBasics, Transform target){
        this.steeringBasics = steeringBasics;
        this.target = target;
    }

    public override void Perform()
    {
        Vector3 accel = steeringBasics.Seek(target.position);
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }
}