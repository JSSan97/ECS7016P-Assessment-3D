using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

// Wrapper for Evade Behaviour, modified based on UnityMovementAI 'UnitEvade' to work with interchangable behaviour
public class BehaviourEvade : CustomBehaviour
{
    public MovementAIRigidbody target;
    SteeringBasics steeringBasics;
    Evade evade;

    public BehaviourEvade(SteeringBasics steeringBasics, Evade evade, MovementAIRigidbody target){
        this.steeringBasics = steeringBasics;
        this.evade = evade;
        this.target = target;
    }

    public override void Perform()
    {
        Vector3 accel = evade.GetSteering(target);
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

}