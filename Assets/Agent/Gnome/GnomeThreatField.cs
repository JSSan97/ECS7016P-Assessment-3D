using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeThreatField : MonoBehaviour
{
    private GameObject pursuer;
    private GameObject gnome;

    private void Awake() {
        this.gnome = this.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Hunter":
                if(pursuer == null) {
                    if(!isObstacleBetweenAgentAndPursuer(other.gameObject))
                        this.pursuer = other.gameObject;
                } else {
                    this.pursuer = SwitchNearestPursuer(pursuer, other);
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other) {   
        switch (other.gameObject.tag) {
            case "Hunter":
                if (pursuer == other.gameObject)
                    this.pursuer = null;
                break;
        }
    }

    private GameObject SwitchNearestPursuer(GameObject targetObject, Collider other) {
        float distance1 = Vector3.Distance(this.gnome.transform.position, other.gameObject.transform.position);
        float distance2 = Vector3.Distance(this.gnome.transform.position, targetObject.transform.position);
        if(distance1 < distance2) {
            if(!isObstacleBetweenAgentAndPursuer(other.gameObject))
                return other.gameObject;
        }

        return targetObject;
    }

    private bool isObstacleBetweenAgentAndPursuer(GameObject pursuer){
        // Agent can't see through pursuer through wall
        RaycastHit hit;
        if (Physics.Linecast(this.gnome.transform.position, pursuer.transform.position, out hit)){
            if(hit.transform.tag == "Wall")
                return true;
        }
        return false;
    }


    public GameObject getPursuer(){
        return this.pursuer;
    }
}
