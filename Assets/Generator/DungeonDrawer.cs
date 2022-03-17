using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDrawer
{
    public GameObject parent;

    public DungeonDrawer(GameObject parent) {
        this.parent = parent;
    }

    public void drawRoom(Node node) {
        Color color = Color.black;
        node.quadSpace = this.createQuad("Room " + node.name, 0.02f, node.roomBottomLeft, node.roomBottomRight, node.roomTopLeft, node.roomTopRight, color);
    }

    public void drawPartition(Node node) {
        Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        node.quadSpace = this.createQuad("Quad " + node.name, 0.01f, node.bottomLeft, node.bottomRight, node.topLeft, node.topRight, randomColor);
    }

    public void drawCorridors(Node node) {
        // A - Horizontal Split Bottom of sibling or Vertical Split Left of Sibling
        // B - Horizontal Split Top of sibling or Vertical Split Right of Sibling
        char lastCharacter = node.name[node.name.Length - 1];
        Node sibling = node.sibling;

        // We only need to draw one corridor from sibling so just use one child (left or 'A')
        if(lastCharacter.Equals('A')) {
            // Find the two corner vectors which have the closest vectors of its siblings
            // Horizontal split
            float distance1 = Vector3.Distance(node.roomTopRight, sibling.roomBottomRight);
            // Vertical split
            float distance2 = Vector3.Distance(node.roomBottomRight, sibling.roomBottomLeft);
        
            Vector3 corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight;

            if (distance1 < distance2) {
                // Horizontal Split, we want to shorten width of corridor
                float minimumX = (sibling.roomBottomLeft.x > node.roomTopLeft.x) ? sibling.roomBottomLeft.x : node.roomTopLeft.x;
                float maximumX = (sibling.roomBottomRight.x < node.roomTopRight.x) ? sibling.roomBottomRight.x : node.roomTopRight.x;

                // Width of the corridor
                float width = 5;
                // + or - width as we need an offset (corridors shouldn't be at far end)
                float xPosition = Random.Range(minimumX + width, maximumX - width);

                corridorBottomLeft = new Vector3(xPosition, node.roomTopLeft.y, node.roomTopLeft.z);
                corridorBottomRight = new Vector3(xPosition + width, node.roomTopRight.y, node.roomTopRight.z);
                corridorTopLeft = new Vector3(xPosition, sibling.roomBottomLeft.y, node.roomBottomLeft.z);
                corridorTopRight = new Vector3(xPosition + width, sibling.roomBottomRight.y, sibling.roomBottomRight.z);
            } else {
                // Vertical split, we want to shorten height of corridor
                float minimumY = (sibling.roomBottomLeft.y > node.roomBottomRight.y) ? sibling.roomBottomLeft.y : node.roomBottomRight.y;
                float maximumY = (sibling.roomTopRight.y < node.roomTopLeft.y) ? sibling.roomTopRight.y : node.roomTopLeft.y;

                // Height of the corridor
                float height = 5;
                // + or - height  as we need an offset (corridors shouldn't be at far end)
                float yPosition = Random.Range(minimumY + height, maximumY - height);

                corridorBottomLeft = new Vector3(node.roomBottomRight.x, yPosition, node.roomBottomRight.z);
                corridorBottomRight = new Vector3(sibling.roomBottomLeft.x, yPosition, sibling.roomBottomLeft.z);
                corridorTopLeft = new Vector3(node.roomTopRight.x, yPosition + height, node.roomTopRight.z);
                corridorTopRight = new Vector3(sibling.roomTopLeft.x, yPosition + height, sibling.roomTopLeft.z);
            }

            string objectName = "Corridor " + node.name + " to " + sibling.name; 
            Color color = Color.black;
            this.createQuad(objectName, 0.02f, corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight, color);
        } 
    }


    public GameObject createQuad(string name, float elevation, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Color color)
    {
        // Create new quad object
        GameObject quad = new GameObject(name);
        // Set the quad object under the pcg parent (can be any game object)
        quad.transform.parent = parent.transform;
        // Rotate 90 degrees to fit in dungeon
        quad.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
        // Elevate the dungeon by y-axis (so no overlap between different quads)
        quad.transform.position = new Vector3(quad.transform.position.x, elevation, quad.transform.position.z);
        // Define Mesh Renderer and set material
        MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        // Define MeshFilter
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        // Define Mesh
        Mesh mesh = new Mesh();
        // Insert corner positions for quad mesh generation
        Vector3[] vertices = new Vector3[4]
        {
            bottomLeft,
            bottomRight,
            topLeft,
            topRight,
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        // Set normals of mesh.
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;
        // Set UVs of mesh
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
        // Set color of quad
        quad.GetComponent<Renderer>().material.color = color;

        return quad;
    }
}