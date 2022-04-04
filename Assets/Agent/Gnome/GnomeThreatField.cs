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
            return other.gameObject;
        } else {
            return targetObject;
        }
    }


    public GameObject getPursuer(){
        return this.pursuer;
    }
}
