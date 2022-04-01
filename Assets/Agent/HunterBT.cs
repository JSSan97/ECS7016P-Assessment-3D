using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using UnityMovementAI;

public class HunterBT : MonoBehaviour
{
    // How fast the wolf moves
    public float speed = 5;
    // The wolf's behaviour tree
    private Root tree;              
    // The wolf's behaviour blackboard  
    private Blackboard blackboard;
    // The current behaviour
    CustomBehaviour behaviour;


    private void Awake() {
        SteeringBasics steeringBasics = GetComponent<SteeringBasics>();
        Wander2 wander = GetComponent<Wander2>();
        behaviour = new CustomWander(steeringBasics, wander);
    }
    

    private void Start()
    {
        // tree = CreateBehaviourTree();
        // blackboard = tree.Blackboard;
        // tree.Start()
    }

    private void FixedUpdate() {
        behaviour.Perform();
    }


    // private Root CreateBehaviourTree()
    // {
    //     return new Root(new RandomSequence(
    //         Wander()
    //     ));
    // }

    // private Node Wander()
    // {
    //     return new Sequence(
    //         new Action(() => Wander(),
    //         new Wait(UnityEngine.Random.Range(0.0f, 5.0f))
    //     );
    // }


}
