using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class CustomFlee : CustomBehaviour
{
    public Transform target;
    SteeringBasics steeringBasics;
    Flee flee;

    public CustomFlee(SteeringBasics steeringBasics, Flee flee, Transform target){
        this.steeringBasics = steeringBasics;
        this.flee = flee;
        this.target = target;
    }

    public override void Perform()
    {
        Vector3 accel = flee.GetSteering(target.position);
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

}
