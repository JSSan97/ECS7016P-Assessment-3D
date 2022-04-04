using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomePerception : MonoBehaviour
{
    private GameObject nearestWaterTile;
    private GameObject nearestGrassTile;

    private GameObject gnome;

    private void Awake() {
        this.gnome = this.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Water":
                if(nearestWaterTile == null) {
                    nearestWaterTile = other.gameObject;
                } else {
                    nearestWaterTile = switchNearestTarget(nearestWaterTile, other);
                }

                break;
            case "Grass":
                if(nearestGrassTile == null) {
                    nearestGrassTile = other.gameObject;
                } else {
                    nearestGrassTile = switchNearestTarget(nearestGrassTile, other);
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other) {   
        switch (other.gameObject.tag) {
            case "Water":
                if(nearestWaterTile == other.gameObject) {
                    nearestWaterTile = null;
                }
                break;
            case "Grass":
                if(nearestGrassTile == other.gameObject) {
                    nearestGrassTile = null;
                }
                break;
        }
    }

    private GameObject switchNearestTarget(GameObject targetObject, Collider other) {
        float distance1 = Vector3.Distance(this.gnome.transform.position, other.gameObject.transform.position);
        float distance2 = Vector3.Distance(this.gnome.transform.position, targetObject.transform.position);
        if(distance1 < distance2) {
            return other.gameObject;
        } else {
            return targetObject;
        }
    }

}
