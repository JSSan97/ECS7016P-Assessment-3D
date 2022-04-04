using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterPerception : MonoBehaviour
{
    private GameObject target;

    private void Update(){
        if (target != null && !target.activeSelf)
            target = null;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Gnome":
                target = other.gameObject;
                break;
        }
    }

    private void OnTriggerStay(Collider other) {   
        switch (other.gameObject.tag) {
            case "Gnome":
                if (target == other.gameObject) {
                    // Gnomes hide in the grass
                    if(target.GetComponent<Gnome>().isTouchingGrass) {
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

    public GameObject getTarget(){
        return this.target;
    }
}
