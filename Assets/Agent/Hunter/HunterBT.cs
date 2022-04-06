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

    private SteeringBasics steeringBasics;
    private Wander2 wander;
    private Pursue pursue;
    private HunterPerception hunterPerception;
    private HunterThreatField hunterThreatField;
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
        tree = CreateBehaviourTree();
        blackboard = tree.Blackboard;
        tree.Start();
    }

    private void FixedUpdate() {
        if(behaviour != null)
            behaviour.Perform();
    }

    private Root CreateBehaviourTree()
    {
        return new Root(
            new Service(0.5f, UpdatePerception,
                new Selector(
                    new BlackboardCondition("isPursued", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeFlee()
                    ),                
                    new BlackboardCondition("hasTarget", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeChaseTarget()
                    ), 
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
