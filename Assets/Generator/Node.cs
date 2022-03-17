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

    // Game Object of Space
    public GameObject quadSpace { get; set; }
    public GameObject quadRoom {get; set; }

    // Name - Helps identify objects in hierarchy, parents and position
    public string name { get; set; }

    // Create unity quads to show partitioning
    QuadCreator QuadCreator = new QuadCreator();

    public Node(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        this.topLeft = topLeft;
        this.topRight = topRight;
    }

    public void updateRoomSpace() {
        float height = Vector3.Distance(this.topRight, this.bottomRight);
        float width = Vector3.Distance(this.bottomLeft, this.bottomRight);
        float offset = 3.0f;

        roomTopLeft = new Vector3(topLeft.x + offset, topLeft.y - offset, topLeft.z);
        roomBottomRight = new Vector3(bottomRight.x - offset, bottomRight.y + offset, bottomRight.z);
        roomTopRight = new Vector3(topRight.x - offset, topRight.y - offset, topRight.z);
        roomBottomLeft = new Vector3(bottomLeft.x + offset, bottomLeft.y + offset, bottomLeft.z);
    }

    public void drawRoom(GameObject pcg) {
        Color color = Color.black;
        this.quadSpace = QuadCreator.createQuad(pcg, "Room " + name, 0.02f, roomBottomLeft, roomBottomRight, roomTopLeft, roomTopRight, color);
    }

    public void drawPartition(GameObject pcg) {
        Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        this.quadSpace = QuadCreator.createQuad(pcg, "Quad " + name, 0.01f, bottomLeft, bottomRight, topLeft, topRight, randomColor);
    }

    public void drawCorridors(GameObject pcg) {
        // A - Horizontal Split Bottom of sibling or Vertical Split Left of Sibling
        // B - Horizontal Split Top of sibling or Vertical Split Right of Sibling
        char lastCharacter = name[name.Length - 1];

        // We only need to draw one corridor from sibling so just use one child (left or 'A')
        if(lastCharacter.Equals('A')) {
            // Find the two corner vectors which have the closest vectors of its siblings
            // Horizontal split
            float distance1 = Vector3.Distance(this.roomTopRight, sibling.roomBottomRight);
            // Vertical split
            float distance2 = Vector3.Distance(this.roomBottomRight, sibling.roomBottomLeft);
        
            Vector3 corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight;

            if (distance1 < distance2) {
                // Horizontal Split, we want to shorten width of corridor
                float minimumX = (sibling.roomBottomLeft.x > this.roomTopLeft.x) ? sibling.roomBottomLeft.x : this.roomTopLeft.x;
                float maximumX = (sibling.roomBottomRight.x < this.roomTopRight.x) ? sibling.roomBottomRight.x : this.roomTopRight.x;

                // Width of the corridor
                float width = 5;
                // + or - width as we need an offset (corridors shouldn't be at far end)
                float xPosition = Random.Range(minimumX + width, maximumX - width);

                corridorBottomLeft = new Vector3(xPosition, this.roomTopLeft.y, this.roomTopLeft.z);
                corridorBottomRight = new Vector3(xPosition + width, this.roomTopRight.y, this.roomTopRight.z);
                corridorTopLeft = new Vector3(xPosition, sibling.roomBottomLeft.y, this.roomBottomLeft.z);
                corridorTopRight = new Vector3(xPosition + width, sibling.roomBottomRight.y, sibling.roomBottomRight.z);
            } else {
                // Vertical split, we want to shorten height of corridor
                float minimumY = (sibling.roomBottomLeft.y > this.roomBottomRight.y) ? sibling.roomBottomLeft.y : this.roomBottomRight.y;
                float maximumY = (sibling.roomTopRight.y < this.roomTopLeft.y) ? sibling.roomTopRight.y : this.roomTopLeft.y;

                // Height of the corridor
                float height = 5;
                // + or - height  as we need an offset (corridors shouldn't be at far end)
                float yPosition = Random.Range(minimumY + height, maximumY - height);

                corridorBottomLeft = new Vector3(this.roomBottomRight.x, yPosition, this.roomBottomRight.z);
                corridorBottomRight = new Vector3(sibling.roomBottomLeft.x, yPosition, sibling.roomBottomLeft.z);
                corridorTopLeft = new Vector3(this.roomTopRight.x, yPosition + height, this.roomTopRight.z);
                corridorTopRight = new Vector3(sibling.roomTopLeft.x, yPosition + height, sibling.roomTopLeft.z);
            }

            string objectName = "Corridor " + name + " to " + sibling.name; 
            Color color = Color.black;
            this.quadSpace = QuadCreator.createQuad(pcg, objectName, 0.02f, corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight, color);
        } 
    }

    public void appendToName(string letter) {
        // Ensure parent has been set before using.
        this.name = parent.name + letter;
    }
}
