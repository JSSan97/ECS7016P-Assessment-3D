using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;

public class GnomeBT : MonoBehaviour
{
    public float speed = 5;
    // The gnome's behaviour tree
    private Root tree;              
    // The gnome's behaviour blackboard  
    private Blackboard blackboard;
    // Current behaviour
    private int currentBehaviour = 1;


}
