using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;

public class GnomeBT : MonoBehaviour
{
    // Current behaviour
    CustomBehaviour behaviour;

    // Gnome
     private Gnome gnome;

    // Fields and sensors that uses onTriggers used to aid in behaviour
    private GnomePerception gnomePerception;
    private GnomeThreatField gnomeThreatField;
    private GnomeFlockingSensor gnomeFlockingSensor;

    // Behaviours using UnityMovementAI library
    private SteeringBasics steeringBasics;
    private Wander2 wander;
    private Evade evade;
    private Flee flee;
    // Flocking behaviours using UnityMovementAI library
    private Cohesion cohesion;
    private Separation separation;
    private VelocityMatch velocityMatch;

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
        separation = GetComponent<Separation>();
        velocityMatch = GetComponent<VelocityMatch>();
        cohesion = GetComponent<Cohesion>();
        gnomePerception = transform.GetChild(0).gameObject.GetComponent<GnomePerception>();
        gnomeThreatField = transform.GetChild(1).gameObject.GetComponent<GnomeThreatField>();
        gnomeFlockingSensor = transform.GetChild(2).gameObject.GetComponent<GnomeFlockingSensor>();
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
        // Main Behaviour Tree
        return new Root(
            new Service(0.1f, UpdatePerception,
                new Selector(
                    // Heal when under a certain threshold of health.
                    new BlackboardCondition("health", Operator.IS_SMALLER_OR_EQUAL, 80.0f, Stops.IMMEDIATE_RESTART,
                        nodeHeal()
                    ), 
                    // Escape/Flee/Hide Hunter when a hunter is nearby.
                    new BlackboardCondition("isHunterNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeEscapeHunter()
                    ),
                    // Quench Thirst behaviour when below a certain threshhold of thirst.
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 50.0f, Stops.IMMEDIATE_RESTART,
                        nodeQuenchThirst()
                    ), 
                    // Stay in water until a threshhold.
                    new BlackboardCondition("thirst", Operator.IS_SMALLER_OR_EQUAL, 80.0f, Stops.IMMEDIATE_RESTART,
                        nodeStayInWater()
                    ), 
                    // Flock to player if nearby.
                    new BlackboardCondition("isPlayerNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeFlock()
                    ), 
                    // Spin near a wall.
                    new BlackboardCondition("isWallInFront", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        nodeSpin()
                    ),
                    // Wander about.
                    nodeWander()
                )
            )
        );
    }

    /**************************************
    * 
    * GNOME ACTIONS
    * 
    * Wander, Seek Water & Grass, Wander, Evade, Flee, Stand, Spin and Flock to Player
    */
    private void actionSeekWater() {
        if(gnomePerception.getPerceivedWaterTile() != null)
            this.behaviour = new BehaviourSeek(this.steeringBasics, gnomePerception.getPerceivedWaterTile().transform);
    }

    private void actionSeekGrass() {
        if(gnomePerception.getPerceivedGrassTile() != null)
            this.behaviour = new BehaviourSeek(this.steeringBasics, gnomePerception.getPerceivedGrassTile().transform);
    }

    private void actionPushHunter() {
        if(gnomeThreatField.getPursuer() != null)
            this.behaviour = new BehaviourSeek(this.steeringBasics, gnomeThreatField.getPursuer().transform);
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

    private void actionFlock() {
        this.behaviour = new BehaviourFlocking(this.steeringBasics, this.wander, this.cohesion, this.separation, this.velocityMatch, this.gnomeFlockingSensor);
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
        blackboard["isPlayerNearby"] = gnomeFlockingSensor.getIsPlayerNearby();
        WallCollisionPerception();
    }

    private void WallCollisionPerception()
    {
        // Detect when there is a wall in front of the gnome.
        bool wallInFront = false;
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(transform.right);
        if (Physics.Raycast(transform.position, fwd, out hit, 1))
            if(hit.collider.gameObject.tag == "Wall" && hit.collider != null) {
                wallInFront = true;
            }

        blackboard["isWallInFront"] = wallInFront;
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

    private Node nodeFlock()
    {
        return new Action(() => actionFlock());
    }

    private Node nodeEscapeHunter()
    {
        // Behaviour when needing to escape hunter
        return new Selector(
            new BlackboardCondition("isTouchingGrass", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeStand()
            ),
            new BlackboardCondition("isGrassNearby", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                nodeSeekGrass()
            ),
            new RandomSelector(
                new Sequence(
                    nodeFlee()
                ),
                new Sequence(
                    nodeEvade()
                )
            )
        );
    }

    private Node nodeHeal()
    {
        // Behaviour when needing to heal
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
        // Behaviour when quenching thirst
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
        // Behaviour for staying in water
        return new BlackboardCondition("isTouchingWater", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
            new Selector(
                new BlackboardCondition("isHunterNearby", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART,
                    nodeStand()
                ),
                nodeWander()
            )
        );
    }
}
