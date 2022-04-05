using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    // The room to which the tile belongs to.
    private BSPNode room; 
    // Ground, Grass, Wall, Water
    private string type; 
    // Position of game object tile
    private Vector3 position; 
    // How heigh the walls should be for wall tiles.
    private float wallHeight;
    // The actual tile came object.
    private GameObject tile;

    public Tile(BSPNode room, string type, Vector3 position, float wallHeight) {
        this.room = room;
        this.type = type;
        this.position = position;
        this.wallHeight = wallHeight;
        CreateTile();
    }

    private void CreateTile() {
        // Create a cube at the position designated and under the room gameobject.
        tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.parent = this.room.quadRoom.transform;
        tile.transform.localPosition = position;

        // For the tile, set the color, name and tag.
        switch (this.type) {
            case "Ground":
                tile.GetComponent<Renderer>().material.color = Settings.groundColor;
                tile.GetComponent<BoxCollider>().size = new Vector3(1, 0.9f, 1);
                tile.name = "Ground Tile";
                tile.tag = "Ground";
                break;
            case "Grass":
                tile.GetComponent<Renderer>().material.color = Settings.grassColor;
                tile.transform.localScale += Vector3.up;
                tile.GetComponent<BoxCollider>().isTrigger = true;
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
                tile.transform.localScale += Vector3.down * 0.1f;
                tile.GetComponent<BoxCollider>().size = new Vector3(1, 2.0f, 1);
                tile.GetComponent<BoxCollider>().isTrigger = true;
                tile.name = "Water Tile";
                tile.tag = "Water";
                break;
        }
    }
}
