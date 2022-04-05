using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class BehaviourFlocking : CustomBehaviour
{
    public float cohesionWeight = 1.5f;
    public float separationWeight = 1f;
    public float velocityMatchWeight = 1f;

    SteeringBasics steeringBasics;
    Wander2 wander;
    Cohesion cohesion;
    Separation separation;
    VelocityMatch velocityMatch;
    GnomeFlockingSensor sensor;

    public BehaviourFlocking(SteeringBasics steeringBasics, Wander2 wander, Cohesion cohesion, Separation separation, VelocityMatch velocityMatch, GnomeFlockingSensor sensor)
    {
        this.steeringBasics = steeringBasics;
        this.wander = wander;
        this.cohesion = cohesion;
        this.separation = separation;
        this.velocityMatch = velocityMatch;
        this.sensor = sensor;
    }

    public override void Perform()
    {
        Vector3 accel = Vector3.zero;

        accel += cohesion.GetSteering(sensor.targets) * cohesionWeight;
        accel += separation.GetSteering(sensor.targets) * separationWeight;
        accel += velocityMatch.GetSteering(sensor.targets) * velocityMatchWeight;

        if (accel.magnitude < 0.005f)
        {
            accel = wander.GetSteering();
        }

        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }
}
