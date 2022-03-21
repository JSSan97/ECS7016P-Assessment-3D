using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata
{
    // Room Width
    private int width;
    // Room Height
    private int height;
    // Coordinates of room
    private int[,] room;
    // Fill Percent
    private int fillPercent;

    public CellularAutomata(Node node) {
        this.height = Mathf.RoundToInt(Vector3.Distance(node.roomTopRight, node.roomBottomRight));
        this.width = Mathf.RoundToInt(Vector3.Distance(node.roomBottomLeft, node.roomBottomRight));
        this.fillPercent = Random.Range(0, 30);
    }

    private void fillRoom() {
        // Random Seed
        string seed = Time.time.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        // Create some noise
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
                    // The walls except the corridor should be
					room[x,y] = 1;
				} else {
                    // Fill the rest of the area
                    room[x, y] = (pseudoRandom.Next(0,100) < fillPercent)? 1: 0;
                }
            }
        }
    }

    private void drawRoom() {
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                Gizmos.color = (room[x, y] == 1) ? Color.black : Color.white;
				Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
				Gizmos.DrawCube(pos,Vector3.one);
            }
        }
    }
}
