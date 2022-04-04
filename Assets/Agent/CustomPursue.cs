using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class CustomPursue : CustomBehaviour
{
    public MovementAIRigidbody target;
    SteeringBasics steeringBasics;
    Pursue pursue;

    public CustomPursue(SteeringBasics steeringBasics, Pursue pursue, MovementAIRigidbody target){
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
