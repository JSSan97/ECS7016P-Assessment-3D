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
    private GnomeThreatField gnomeThreatField;
    private SteeringBasics steeringBasics;
    private Wander2 wander;
    private Flee flee;

    // The gnome's behaviour tree
    private Root tree;             
    // The gnome's behaviour blackboard     
    private Blackboard blackboard;

    private void Awake() {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        gnome = GetComponent<Gnome>();
        flee = GetComponent<Flee>();
        gnomePerception = transform.GetChild(0).gameObject.GetComponent<GnomePerception>();
        gnomeThreatField = transform.GetChild(1).gameObject.GetComponent<GnomeThreatField>();
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
                    new BlackboardCondition("isHunterNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeEscapeHunter()
                    ),
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 60.0f, Stops.IMMEDIATE_RESTART,
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
    * Wander, Seek Water, Hide in Bush, Flee
    */
    private void actionSeekWater() {
        if(gnomePerception.getNearestWaterTile() != null)
            this.behaviour = new CustomSeek(this.steeringBasics, gnomePerception.getNearestWaterTile().transform);
    }

    private void actionSeekGrass() {
        if(gnomePerception.getNearestGrassTile() != null)
            this.behaviour = new CustomSeek(this.steeringBasics, gnomePerception.getNearestGrassTile().transform);
    }

    private void actionWander() {
        this.behaviour = new CustomWander(this.steeringBasics, this.wander);
    }

    private void actionFlee() {
        this.behaviour = new CustomFlee(this.steeringBasics, this.flee, gnomeThreatField.getPursuer().transform);
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
        bool isWaterNearby = false;
        
        if(gnomePerception.getNearestWaterTile() != null){
            isWaterNearby = true;
            RaycastHit hit;
            // We don't want to seek water when there are walls in the way
            if (Physics.Linecast(this.gnome.transform.position, gnomePerception.getNearestWaterTile().transform.position, out hit)){
                if(hit.transform.tag == "Wall")
                    isWaterNearby = false;
            }
        }

        bool isGrassNearby = false;
        if(gnomePerception.getNearestGrassTile() != null){
            isGrassNearby = true;
            RaycastHit hit;
            // We don't want to seek water when there are walls in the way
            if (Physics.Linecast(this.gnome.transform.position, gnomePerception.getNearestGrassTile().transform.position, out hit)){
                if(hit.transform.tag == "Wall")
                    isGrassNearby = false;
            }
        }


        blackboard["thirst"] = gnome.thirst;
        blackboard["isTouchingWater"] = gnome.isTouchingWater;
        blackboard["isWaterNearby"] = isWaterNearby;

        blackboard["isHunterNearby"] = gnomeThreatField.getPursuer() != null;
        blackboard["isTouchingGrass"] = gnome.isTouchingWater;
        blackboard["isGrassNearby"] = isGrassNearby;

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

    private Node nodeSeekGrass()
    {
        return new Action(() => actionSeekGrass());
    }

    private Node nodeFlee()
    {
        return new Action(() => actionFlee());
    }

    private Node nodeEscapeHunter()
    {
        return new Selector(
            new BlackboardCondition("isGrassNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeSeekGrass()
            ),
            nodeSeekGrass()
        );
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
