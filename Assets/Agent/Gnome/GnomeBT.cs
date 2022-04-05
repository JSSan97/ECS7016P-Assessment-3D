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
    private Evade evade;
    private Flee flee;

    // The gnome's behaviour tree
    private Root tree;             
    // The gnome's behaviour blackboard     
    private Blackboard blackboard;

    private Rigidbody rigidBody;

    private void Awake() {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        gnome = GetComponent<Gnome>();
        flee = GetComponent<Flee>();
        evade = GetComponent<Evade>();
        rigidBody = GetComponent<Rigidbody>();
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
            new Service(0.1f, UpdatePerception,
                new Selector(
                    new BlackboardCondition("health", Operator.IS_SMALLER_OR_EQUAL, 99.0f, Stops.IMMEDIATE_RESTART,
                        nodeHeal()
                    ), 
                    new BlackboardCondition("isHunterNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeEscapeHunter()
                    ),
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 60.0f, Stops.IMMEDIATE_RESTART,
                        nodeQuenchThirst()
                    ), 
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 80.0f, Stops.IMMEDIATE_RESTART,
                        nodeStayInWater()
                    ), 
                    // new BlackboardCondition("wallInFront", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                    //     nodeSpin()
                    // ), 
                    nodeWander()
                )
            )
        );
    }

    /**************************************
    * 
    * GNOME ACTIONS
    * 
    * Wander, Seek Water, Seek Grass, Wander, Evade and Flee
    */
    private void actionSeekWater() {
        if(gnomePerception.getPerceivedWaterTile() != null)
            this.behaviour = new BehaviourSeek(this.steeringBasics, gnomePerception.getPerceivedWaterTile().transform);
    }

    private void actionSeekGrass() {
        if(gnomePerception.getPerceivedGrassTile() != null)
            this.behaviour = new BehaviourSeek(this.steeringBasics, gnomePerception.getPerceivedGrassTile().transform);
    }

    private void actionWander() {
        this.behaviour = new BehaviourWander(this.steeringBasics, this.wander);
    }

    private void actionFlee() {
        this.behaviour = new BehaviourFlee(this.steeringBasics, this.flee, gnomeThreatField.getPursuer().transform);
    }

    private void actionEvade() {
        this.behaviour = new BehaviourEvade(this.steeringBasics, this.evade, gnomeThreatField.getPursuer().GetComponent<MovementAIRigidbody>());
    }

    private void actionStand() {
        this.behaviour = null;
    }

    private void actionSpin() {
        this.behaviour = new BehaviourSpin(this.steeringBasics, this.rigidBody);
    }


    /**************************************
    * 
    * GNOME PERCEPTION
    * 
    */
    private void UpdatePerception()
    {
        blackboard["thirst"] = gnome.thirst;
        blackboard["health"] = gnome.health;
        blackboard["isTouchingWater"] = gnome.isTouchingWater;
        blackboard["isTouchingGrass"] = gnome.isTouchingGrass;
        blackboard["isWaterNearby"] = gnomePerception.getPerceivedWaterTile() != null;

        blackboard["isHunterNearby"] = gnomeThreatField.getPursuer() != null;
        blackboard["isTouchingGrass"] = gnome.isTouchingWater;
        blackboard["isGrassNearby"] = gnomePerception.getPerceivedGrassTile() != null;
        WallCollisionPerception();
    }

    private void WallCollisionPerception()
    {
        bool wallInFront = false;
        // Bit shift the index of the layer (3) to get a bit mask
        int layerMask = 3;

        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, out hit, 1))
            if(hit.collider.gameObject.tag == "Wall" && hit.collider != null) {
                wallInFront = true;
            }

        blackboard["wallInFront"] = wallInFront;
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

    private Node nodeEvade()
    {
        return new Action(() => actionEvade());
    }

    private Node nodeSpin()
    {
        return new Action(() => actionSpin());
    }

    private Node nodeEscapeHunter()
    {
        return new Selector(
            new BlackboardCondition("isTouchingGrass", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeStand()
            ),
            new BlackboardCondition("isGrassNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeSeekGrass()
            ),
            new RandomSelector(
                nodeFlee(),
                nodeEvade()
            )
        );
    }

    private Node nodeHeal()
    {
        return new Selector(
            new BlackboardCondition("isTouchingGrass", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    nodeStand(),
                    new Wait(3.0f)
                )
            ), 
            new BlackboardCondition("isGrassNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeSeekGrass()
            ),
            nodeWander()
        );
    }

    private Node nodeQuenchThirst()
    {
        return new Selector(
            new BlackboardCondition("isTouchingWater", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeStayInWater()
            ), 
            new BlackboardCondition("isWaterNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeSeekWater()
            ),
            nodeWander()
        );
    }

    private Node nodeStayInWater(){
        return new BlackboardCondition("isTouchingWater", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
            new BlackboardCondition("isHunterNearby", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART,
                nodeStand()
            )
        );
    }
}
