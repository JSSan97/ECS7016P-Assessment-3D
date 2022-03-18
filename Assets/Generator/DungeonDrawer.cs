using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDrawer
{
    public GameObject parent;

    private GameObject partitions;
    private GameObject rooms;
    private GameObject corridors;
    private float corridorWidth;

    public DungeonDrawer(GameObject parent, float corridorWidth) {
        this.parent = parent;
        // Create parent objects to organise in hierarchy
        this.partitions = new GameObject("Partitions");
        this.rooms = new GameObject("Rooms");
        this.corridors = new GameObject("Corridors");

        partitions.transform.parent = parent.transform;
        rooms.transform.parent = parent.transform;
        corridors.transform.parent = parent.transform;
        this.corridorWidth = corridorWidth;
    }

    public void drawRoom(Node node) {
        Color color = Color.black;
        node.quadSpace = this.createQuad(this.rooms, "Room " + node.name, 0.02f, node.roomBottomLeft, node.roomBottomRight, node.roomTopLeft, node.roomTopRight, color);
    }

    public void drawPartition(Node node) {
        Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        node.quadSpace = this.createQuad(this.partitions, "Quad " + node.name, 0.01f, node.bottomLeft, node.bottomRight, node.topLeft, node.topRight, randomColor);
    }

    public void drawCorridors(Node node, Node node2) {
        // Find the two corner vectors which have the closest vectors of its siblings
        // Horizontal split
        float distance1 = Vector3.Distance(node.roomTopRight, node2.roomBottomRight);
        // Vertical split
        float distance2 = Vector3.Distance(node.roomBottomRight, node2.roomBottomLeft);
    
        Vector3 corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight;

        if (distance1 < distance2) {
            // Horizontal Split, we want to shorten width of corridor
            float minimumX = (node2.roomBottomLeft.x > node.roomTopLeft.x) ? node2.roomBottomLeft.x : node.roomTopLeft.x;
            float maximumX = (node2.roomBottomRight.x < node.roomTopRight.x) ? node2.roomBottomRight.x : node.roomTopRight.x;

            // X Position of corridor
            float xPosition = Random.Range(minimumX, maximumX - this.corridorWidth);

            // Corners to draw quad
            corridorBottomLeft = new Vector3(xPosition, node.roomTopLeft.y, node.roomTopLeft.z);
            corridorBottomRight = new Vector3(xPosition + this.corridorWidth, node.roomTopRight.y, node.roomTopRight.z);
            corridorTopLeft = new Vector3(xPosition, node2.roomBottomLeft.y, node.roomBottomLeft.z);
            corridorTopRight = new Vector3(xPosition + this.corridorWidth, node2.roomBottomRight.y, node2.roomBottomRight.z);
        } else {
            // Vertical split, we want to shorten height of corridor
            float minimumY = (node2.roomBottomLeft.y > node.roomBottomRight.y) ? node2.roomBottomLeft.y : node.roomBottomRight.y;
            float maximumY = (node2.roomTopRight.y < node.roomTopLeft.y) ? node2.roomTopRight.y : node.roomTopLeft.y;

            // Y position of corridor
            float yPosition = Random.Range(minimumY, maximumY - this.corridorWidth);

            // Corners to draw quad
            corridorBottomLeft = new Vector3(node.roomBottomRight.x, yPosition, node.roomBottomRight.z);
            corridorBottomRight = new Vector3(node2.roomBottomLeft.x, yPosition, node2.roomBottomLeft.z);
            corridorTopLeft = new Vector3(node.roomTopRight.x, yPosition + this.corridorWidth, node.roomTopRight.z);
            corridorTopRight = new Vector3(node2.roomTopLeft.x, yPosition + this.corridorWidth, node2.roomTopLeft.z);
        }

        string objectName = "Corridor " + node.name + " to " + node2.name; 
        Color color = Color.black;
        node.quadCorridor = this.createQuad(this.corridors, objectName, 0.02f, corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight, color);
    }

    private GameObject createQuad(GameObject parentObj, string name, float elevation, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Color color)
    {
        // Create new quad object
        GameObject quad = new GameObject(name);
        // Set the quad object under the pcg parent (can be any game object)
        quad.transform.parent = parentObj.transform;
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