using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;

public class GnomeBT : MonoBehaviour
{
    // Current behaviour
    CustomBehaviour behaviour;

    private Gnome gnome;
    private GnomePerception gnomePerception;
    private SteeringBasics steeringBasics;
    private Wander2 wander;

    // The gnome's behaviour tree
    private Root tree;             
    // The gnome's behaviour blackboard     
    private Blackboard blackboard;      

    private void Awake() {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        gnome = GetComponent<Gnome>();
        gnomePerception = transform.GetChild(0).gameObject.GetComponent<GnomePerception>();
       // actionWander();
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
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 75.0f, Stops.IMMEDIATE_RESTART,
                        nodeDrinkWater()
                    ), 
                    new Sequence(nodeWander())
                )
            )
        );
    }


    /**************************************
    * 
    * GNOME ACTIONS
    * 
    * Wander, Seek Water, Hide, Flee
    */
    private void actionSeekWater() {
        if(gnomePerception.nearestWaterTile != null)
            this.behaviour = new CustomSeek(this.steeringBasics, gnomePerception.nearestWaterTile.transform);
    }

    private void actionWander() {
        this.behaviour = new CustomWander(this.steeringBasics, this.wander);
    }

    private void actionHide() {
        // this.behaviour = new CustomSeek(this.steeringBasics, grass.transform);
    }

    private void actionStand() {
        this.behaviour = null;
    }


    /**************************************
    * 
    * GNOME PERCEPTION
    * 
    */
    private void UpdatePerception()
    {
        blackboard["isTouchingWater"] = gnome.isTouchingWater;
        blackboard["isWaterNearby"] = gnomePerception.nearestWaterTile != null;
        blackboard["thirst"] = gnome.thirst;
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

    private Node nodeStand()
    {
        return new Action(() => actionStand());
    }

    private Node nodeSeekWater()
    {
        return new Action(() => actionSeekWater());
    }

    private Node nodeDrinkWater()
    {
        return new Selector(
            new BlackboardCondition("isTouchingWater", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeStand()
            ), 
            new Selector(
                new BlackboardCondition("isWaterNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                    nodeSeekWater()
                ),
                nodeWander()
            )
        );
    }


}
