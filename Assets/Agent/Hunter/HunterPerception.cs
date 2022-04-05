using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterPerception : MonoBehaviour
{
    private GameObject target;
    private GameObject hunter;

    private void Awake() {
        this.hunter = this.transform.parent.gameObject;
    }

    private void Update(){
        // If the target has died then set target to null
        if (target != null && !target.activeSelf)
            target = null;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Gnome":
                if(!isObstacleBetweenAgentAndTarget(other.gameObject))
                    target = other.gameObject;
                break;
        }
    }

    private void OnTriggerStay(Collider other) {   
        switch (other.gameObject.tag) {
            case "Gnome":
                if (target == other.gameObject) {
                    // Gnomes hide in the grass
                    if(target.GetComponent<Gnome>().isTouchingGrass || isObstacleBetweenAgentAndTarget(target)) {
                        target = null;
                    }
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other) {   
        switch (other.gameObject.tag) {
            case "Gnome":
                if (target == other.gameObject)
                    target = null;
                break;
        }
    }

    private bool isObstacleBetweenAgentAndTarget(GameObject target){
        // We don't want to seek a tile when there is a wall in the way
        RaycastHit hit;
        if (Physics.Linecast(this.hunter.transform.position, target.transform.position, out hit)){
            if(hit.transform.gameObject.tag == "Wall")
                return true;
        }
        return false;
    }

    public GameObject getTarget(){
        return this.target;
    }
    
}
