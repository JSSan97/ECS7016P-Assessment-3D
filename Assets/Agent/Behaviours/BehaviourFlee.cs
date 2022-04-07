using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

// Wrapper for Flee Behaviour, modified based on UnityMovementAI 'UnitFlee' to work with interchangable behaviour
public class BehaviourFlee : CustomBehaviour
{
    public Transform target;
    SteeringBasics steeringBasics;
    Flee flee;

    public BehaviourFlee(SteeringBasics steeringBasics, Flee flee, Transform target){
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
