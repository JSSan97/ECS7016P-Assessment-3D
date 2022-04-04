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
    private CustomBehaviour behaviour;

    private SteeringBasics steeringBasics;
    private Wander2 wander;

    private void Awake()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        behaviour = new CustomWander(steeringBasics, wander);
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
