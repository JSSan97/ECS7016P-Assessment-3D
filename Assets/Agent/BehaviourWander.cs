using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class BehaviourWander : CustomBehaviour
{
    SteeringBasics steeringBasics;
    Wander2 wander;

    public BehaviourWander(SteeringBasics steeringBasics, Wander2 wander){
        this.steeringBasics = steeringBasics;
        this.wander = wander;
    }

    public override void Perform()
    {
        Vector3 accel = wander.GetSteering();
        this.steeringBasics.Steer(accel);
        this.steeringBasics.LookWhereYoureGoing();
    }
}


