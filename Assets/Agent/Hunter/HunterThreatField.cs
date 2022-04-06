using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterThreatField : MonoBehaviour
{
    private GameObject pursuer;
    private GameObject hunter;

    private void Awake() {
        this.hunter = this.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Player":
                pursuer = other.gameObject;
                break;
        }
    }

    private void OnTriggerExit(Collider other) {   
        switch (other.gameObject.tag) {
            case "Player":
                pursuer = null;
                break;
        }
    }

    public GameObject getPursuer(){
        return this.pursuer;
    }
}
