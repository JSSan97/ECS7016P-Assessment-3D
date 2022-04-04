using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    public float health = 100;
    public float thirst = 50;
    public GameObject currentRoom;

    private bool isTouchingWater = false;
    private bool isTouchingGrass = false;

    private void Update() {
        if(isTouchingWater){
            if (this.thirst != 100.0f)
                this.thirst = this.thirst + (2 * Time.deltaTime);
        } else {
            this.thirst = this.thirst + (0.33f * Time.deltaTime);
        }

        if (this.thirst != 0.0f)
            Destroy(this);

        if(isTouchingGrass){
            if (this.health != 100.0f)
                this.health = this.health + (2 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {   
        switch (other.gameObject.tag) {
            case "Water":
                isTouchingWater = true;
                break;
            case "Grass":
                isTouchingGrass = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other) {
        switch (other.gameObject.tag) {
            case "Water":
                isTouchingWater = false;
                break;
            case "Grass":
                isTouchingGrass = false;
                break;
        }
    }
}
