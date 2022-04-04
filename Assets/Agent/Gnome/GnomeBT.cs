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
        actionWander();
    }

    private void FixedUpdate() {
        behaviour.Perform();
    }

    /**************************************
    * 
    * GNOME ACTIONS
    * 
    * Wander, Seek Water, Hide, Flee
    */
    private void actionSeekWater(GameObject water) {
        this.behaviour = new CustomSeek(this.steeringBasics, water.transform);
    }

    private void actionWander() {
        this.behaviour = new CustomWander(this.steeringBasics, this.wander);
    }

    private void actionHide(GameObject grass) {
        this.behaviour = new CustomSeek(this.steeringBasics, grass.transform);
    }



    /**************************************
    * 
    * GNOME PERCEPTION
    * 
    */
    private void UpdatePerception()
    {
        // Vector3 targetPos = TargetTransform().position;
        // Vector3 localPos = this.transform.InverseTransformPoint(targetPos);
        // Vector3 heading = localPos.normalized;
        // blackboard["targetDistance"] = localPos.magnitude;
        // blackboard["targetInFront"] = heading.z > 0;
        // blackboard["targetOnRight"] = heading.x > 0;
        // blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
    }





}
