using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

// Wrapper for Pursue Behaviour, modified based on UnityMovementAI 'UnitPursue' to work with interchangable behaviour
public class BehaviourPursue : CustomBehaviour
{
    public MovementAIRigidbody target;
    SteeringBasics steeringBasics;
    Pursue pursue;

    public BehaviourPursue(SteeringBasics steeringBasics, Pursue pursue, MovementAIRigidbody target){
        this.steeringBasics = steeringBasics;
        this.pursue = pursue;
        this.target = target;
    }

    public override void Perform()
    {
        Vector3 accel = pursue.GetSteering(target);
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }
}
