using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    public float health = 100;
    public float thirst = 50;

    public float thirstDecayPerSecond = 0.5f;
    public float thirstHealPerSecond = 2.0f;

    public bool isTouchingWater = false;
    public bool isTouchingGrass = false;

    private void Update() {
        if(isTouchingWater){
            if (this.thirst <= 100.0f)
                this.thirst += thirstHealPerSecond * Time.deltaTime;
        } else {
            this.thirst -= thirstDecayPerSecond * Time.deltaTime;
        }

        if (this.thirst == 0.0f)
            Destroy(this.gameObject);

        if(isTouchingGrass){
            if (this.health != 100.0f)
                this.health += 2 * Time.deltaTime;
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

    private void OnTriggerStay(Collider other) {   
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
