using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomePerception : MonoBehaviour
{
    // Class to find nearest water and grass tiles
    private GameObject perceivedWaterTile;
    private GameObject perceivedGrassTile;
    private GameObject gnome;

    private void Awake() {
        this.gnome = this.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Water":
                if(perceivedWaterTile == null) {
                    if(!isObstacleBetweenAgentAndTarget(other.gameObject))
                        perceivedWaterTile = other.gameObject;
                } else {
                    perceivedWaterTile = SwitchNearestTarget(perceivedWaterTile, other);
                }

                break;
            case "Grass":
                if(perceivedGrassTile == null) {
                    if(!isObstacleBetweenAgentAndTarget(other.gameObject))
                        perceivedGrassTile = other.gameObject;
                } else {
                    perceivedGrassTile = SwitchNearestTarget(perceivedGrassTile, other);
                }
                break;
        }
    }

    private void OnTriggerStay(Collider other) {
        switch (other.gameObject.tag) {
            case "Water":
                if(perceivedWaterTile == other.gameObject){
                    if(isObstacleBetweenAgentAndTarget(other.gameObject))
                        perceivedWaterTile = null;
                }
                break;
            case "Grass":
                if(perceivedGrassTile == other.gameObject){
                    if(isObstacleBetweenAgentAndTarget(other.gameObject))
                        perceivedGrassTile = null;
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other) {   
        switch (other.gameObject.tag) {
            case "Water":
                if(perceivedWaterTile == other.gameObject) {
                    perceivedWaterTile = null;
                }
                break;
            case "Grass":
                if(perceivedGrassTile == other.gameObject) {
                    perceivedGrassTile = null;
                }
                break;
        }
    }

    private bool isObstacleBetweenAgentAndTarget(GameObject tile){
        // We don't want to seek a tile when there is a wall in the way
        RaycastHit hit;
        if (Physics.Linecast(this.gnome.transform.position, tile.transform.position, out hit)){
            if(hit.transform.gameObject.tag == "Wall")
                return true;
        }
        return false;
    }

    private GameObject SwitchNearestTarget(GameObject targetObject, Collider other) {
        // Always find the nearest target object
        float distance1 = Vector3.Distance(this.gnome.transform.position, other.gameObject.transform.position);
        float distance2 = Vector3.Distance(this.gnome.transform.position, targetObject.transform.position);
        if(distance1 < distance2) {
            if (!isObstacleBetweenAgentAndTarget(other.gameObject))
                return other.gameObject;
        }
        if(!isObstacleBetweenAgentAndTarget(targetObject))
            return targetObject;
        return null;
    }

    public GameObject getPerceivedWaterTile(){
        return this.perceivedWaterTile;
    }

    public GameObject getPerceivedGrassTile(){
        return this.perceivedGrassTile;
    }

}
