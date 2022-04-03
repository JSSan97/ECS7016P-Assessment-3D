using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;

public class GnomeBT : MonoBehaviour
{
    public float speed = 5;
    // The gnome's behaviour tree
    private Root tree;              
    // The gnome's behaviour blackboard  
    private Blackboard blackboard;
    // Current behaviour
    CustomBehaviour behaviour;

    private SteeringBasics steeringBasics;
    private Wander2 wander;


    private void Awake() {
        SteeringBasics steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        behaviour = new CustomWander(steeringBasics, wander);
    }

    private void FixedUpdate() {
        behaviour.Perform();
    }
}
