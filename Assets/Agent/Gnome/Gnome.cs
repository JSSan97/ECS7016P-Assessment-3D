using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    public float health = 100;
    public float thirst = 50;

    // Thirst decreases per second regardless of tile
    public float thirstDecayPerSecond = 0.5f;
    // Thirst increases on water tile
    public float thirstHealPerSecond = 4.0f;
    // Health heals on grass tile
    public float healthHealPerSecond = 4.0f;
    // Health decreases when colliding with hunter per second
    public float hunterDamage = 10.0f;

    public bool isTouchingWater = false;
    public bool isTouchingGrass = false;
    public bool isTouchingHunter = false;

    private void Update() {
        if (this.thirst <= 0.0f || this.health <= 0.0f) {
            Debug.Log(this.gameObject.name + " has died!");
            this.gameObject.SetActive(false);
        }

        if(isTouchingWater){
            if (this.thirst <= 100.0f)
                this.thirst += thirstHealPerSecond * Time.deltaTime;
        } else {
            this.thirst -= thirstDecayPerSecond * Time.deltaTime;
        }

        if(isTouchingGrass){
            if (this.health != 100.0f)
                this.health += healthHealPerSecond * Time.deltaTime;
        }

        if(isTouchingHunter){
            this.health -= hunterDamage * Time.deltaTime;
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

    private void OnCollisionEnter(Collision other) {
        switch (other.gameObject.tag) {
            case "Hunter":
                Debug.Log(this.gameObject.name + " is being attacked by " + other.gameObject.name);
                isTouchingHunter = true;
                break;
        }
    }

    private void OnCollisionExit(Collision other) {
        switch (other.gameObject.tag) {
            case "Hunter":
                isTouchingHunter = false;
                break;
        }
    }

    private void OnCollisionStay(Collision other) {   
        switch (other.gameObject.tag) {
            case "Hunter":
                isTouchingHunter = true;
                break;
        }
    }

}
