using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    enum tileTypes {Ground, Grass, Wall}; 

    Node room;
    string type;
    Vector3 position;
    float wallHeight;

    // Colors
    private Color wallColor = new Color(33.0f/255.0f, 3.0f/255.0f, 1.0f/255.0f, 1.0f);
    private Color groundColor = new Color(71.0f/255.0f, 55.0f/255.0f, 54.0f/255.0f, 1.0f);
    private Color grassColor = new Color(33.0f/255.0f, 180.0f/255.0f, 40.0f/255.0f, 1.0f);

    public Tile(Node room, string type, Vector3 position, float wallHeight) {
        this.room = room;
        this.type = type;
        this.position = position;
        this.wallHeight = wallHeight;
        CreateTile();
    }

    private void CreateTile() {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = this.room.quadRoom.transform;
        cube.transform.localPosition = position;

        switch (this.type) {
            case "Ground":
                cube.GetComponent<Renderer>().material.color = groundColor;
                cube.name = "Ground Tile";
                cube.tag = "Ground";
                break;
            case "Grass":
                cube.GetComponent<Renderer>().material.color = grassColor;
                cube.name = "Grass Tile";
                cube.tag = "Grass";
                break;
            case "Wall":
                cube.transform.localScale += Vector3.up * wallHeight;
                cube.GetComponent<Renderer>().material.color = wallColor;
                cube.name = "Wall Tile";
                cube.tag = "Wall";
                break;
        }
    }

}
