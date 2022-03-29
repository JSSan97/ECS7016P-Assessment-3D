using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Node room; 
    private string type; // Ground, Grass, Wall, Water
    private Vector3 position; // Position of game object
    private float wallHeight;
    private GameObject tile;

    public Tile(Node room, string type, Vector3 position, float wallHeight) {
        this.room = room;
        this.type = type;
        this.position = position;
        this.wallHeight = wallHeight;
        CreateTile();
    }

    private void CreateTile() {
        tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.parent = this.room.quadRoom.transform;
        tile.transform.localPosition = position;

        switch (this.type) {
            case "Ground":
                tile.GetComponent<Renderer>().material.color = Settings.groundColor;
                tile.name = "Ground Tile";
                tile.tag = "Ground";
                break;
            case "Grass":
                tile.GetComponent<Renderer>().material.color = Settings.grassColor;
                tile.name = "Grass Tile";
                tile.tag = "Grass";
                break;
            case "Wall":
                tile.transform.localScale += Vector3.up * wallHeight;
                tile.GetComponent<Renderer>().material.color = Settings.wallColor;
                tile.name = "Wall Tile";
                tile.tag = "Wall";
                break;
            case "Water":
                tile.GetComponent<Renderer>().material.color = Settings.waterColor;
                tile.name = "Water Tile";
                tile.tag = "Water";
                break;
        }
    }
}
