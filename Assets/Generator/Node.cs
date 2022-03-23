using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Nodes represent a partitioned cell.
/  Contains information about the space of the cell and a room.
*/
public class Node
{ 
    // Parent
    public Node parent;

    // Sibling
    public Node sibling;

    // Left and right children
    public Node leftNode { get; set; }
    public Node rightNode { get; set; }

    // Initial Space allowed
    public Vector3 bottomLeft  { get; set; }
    public Vector3 bottomRight  { get; set; }
    public Vector3 topLeft  { get; set; }
    public Vector3 topRight { get; set; }

    // Room Space allowed
    public Vector3 roomBottomLeft  { get; set; }
    public Vector3 roomBottomRight  { get; set; }
    public Vector3 roomTopLeft  { get; set; }
    public Vector3 roomTopRight { get; set; }

    // Keep an array of corridors ends
    public List<List<Vector3>> corridorExits = new List<List<Vector3>>();

    // Game Object of Space
    public GameObject quadSpace { get; set; }
    public GameObject quadRoom { get; set; }
    public GameObject quadCorridor { get; set; }

    // Name - Helps identify objects in hierarchy, parents and position
    public string name { get; set; }

    // Depth of node
    public int depth { get; set; } = 0;

    public Node(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        this.topLeft = topLeft;
        this.topRight = topRight;
    }

    public void UpdateRoomSpace() {
        float height = Vector3.Distance(this.topRight, this.bottomRight);
        float width = Vector3.Distance(this.bottomLeft, this.bottomRight);
        float offset = 3.0f;

        roomTopLeft = new Vector3(topLeft.x + offset, topLeft.y, topLeft.z - offset);
        roomBottomRight = new Vector3(bottomRight.x - offset, bottomRight.y, bottomRight.z + offset);
        roomTopRight = new Vector3(topRight.x - offset, topRight.y, topRight.z - offset);
        roomBottomLeft = new Vector3(bottomLeft.x + offset, bottomLeft.y, bottomLeft.z + offset);
    }

    public void AppendToName(string letter) {
        // Ensure parent has been set before using.
        this.name = parent.name + letter;
    }

    public void UpdateDepth() {
        if(parent != null)
            this.depth = parent.depth + 1;
    }

    public Vector3 GetRoomCentre() {
        return (roomTopLeft + roomBottomRight) / 2;
    }
}
