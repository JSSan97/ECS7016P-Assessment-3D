using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;

public class HunterBT : MonoBehaviour
{
    // The wolf's behaviour tree
    private Root tree;              
    // The wolf's behaviour blackboard  
    private Blackboard blackboard;
    // The current behaviour
    private CustomBehaviour behaviour;

    // Fields and sensors that uses onTriggers used to aid in behaviour
    private HunterPerception hunterPerception;
    private HunterThreatField hunterThreatField;

    // Steering Behaviour from UnityMovementAI
    private SteeringBasics steeringBasics;
    private Wander2 wander;
    private Pursue pursue;
    private Flee flee;

    private void Awake()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        pursue = GetComponent<Pursue>();
        flee = GetComponent<Flee>();
        hunterPerception = transform.GetChild(0).gameObject.GetComponent<HunterPerception>();
        hunterThreatField = transform.GetChild(1).gameObject.GetComponent<HunterThreatField>();
    }

    private void Start()
    {
        // Create Behaviour Tree
        tree = CreateBehaviourTree();
        blackboard = tree.Blackboard;
        tree.Start();
    }

    private void FixedUpdate() {
        // Perform steering every fixed update
        if(behaviour != null)
            behaviour.Perform();
    }

    private Root CreateBehaviourTree()
    {
        // Main behaviour tree
        return new Root(
            new Service(0.5f, UpdatePerception,
                new Selector(
                    // Flee from player if being persued
                    new BlackboardCondition("isPursued", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeFlee()
                    ),            
                    // Otherwise chase target
                    new BlackboardCondition("hasTarget", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeChaseTarget()
                    ), 
                    // Wander
                    new Sequence(nodeWander())
                )
            )
        );
    }

    /**************************************
    * 
    * Hunter
    * 
    * Wander, Chase/Pursue, Flee from player
    */
    private void actionWander() {
        this.behaviour = new BehaviourWander(this.steeringBasics, this.wander);
    }

    private void actionPursue() {
        MovementAIRigidbody target = hunterPerception.getTarget().GetComponent<MovementAIRigidbody>();
        this.behaviour = new BehaviourPursue(this.steeringBasics, this.pursue, target);
    }

    private void actionFlee() {
        this.behaviour = new BehaviourFlee(this.steeringBasics, this.flee, hunterThreatField.getPursuer().transform);
    }

    /**************************************
    * 
    * Hunter PERCEPTION
    * 
    */
    private void UpdatePerception()
    {
        blackboard["hasTarget"] = hunterPerception.getTarget() != null;
        blackboard["isPursued"] = hunterThreatField.getPursuer() != null;
    }

    /**************************************
    * 
    * BEHAVIOUR TREES
    * 
    */
    private Node nodeWander()
    {
        return new Action(() => actionWander());
    }

    private Node nodeChaseTarget()
    {
        return new Action(() => actionPursue());
    }

    private Node nodeFlee()
    {
        return new Action(() => actionFlee());
    }


}
