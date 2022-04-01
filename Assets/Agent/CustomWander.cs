using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class CustomWander : CustomBehaviour
{
    SteeringBasics steeringBasics;
    Wander2 wander;

    public CustomWander(SteeringBasics steeringBasics, Wander2 wander){
        this.steeringBasics = steeringBasics;
        this.wander = wander;
    }

    public override void Perform()
    {
        Vector3 accel = wander.GetSteering();
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }
}


