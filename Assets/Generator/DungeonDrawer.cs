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
    private float wallHeight;

    // Class that draws partitions, rooms, corridors (but does not populate the rooms)
    public DungeonDrawer(GameObject parent, float corridorWidth, float wallHeight) {
        this.parent = parent;
        // Create parent objects to organise in hierarchy
        this.partitions = new GameObject("Partitions");
        this.rooms = new GameObject("Rooms");
        this.corridors = new GameObject("Corridors");
        partitions.transform.parent = parent.transform;
        rooms.transform.parent = parent.transform;
        corridors.transform.parent = parent.transform;
        // For drawing the corridor, how high do we want the walls to be and how wide.
        this.corridorWidth = corridorWidth;
        this.wallHeight = wallHeight;
    }

    public int[,] PopulateRoom(BSPNode node) {
        // Populate room with tiles.
        CellularAutomata cellularAutomata = new CellularAutomata(node);
        cellularAutomata.PopulateRoom(this.wallHeight);

        return cellularAutomata.getWallMap();
    }

    public void DrawRoom(BSPNode node) {
        // Draw a quad for the room, technically this isn't necessary once the rooms are populated
        // but useful to see the generated room space you have (and for debugging).
        node.quadRoom = this.CreateQuad(this.rooms, "Room " + node.name, 0.02f, node.roomBottomLeft, node.roomBottomRight, node.roomTopLeft, node.roomTopRight, Settings.groundColor);
    }

    public void DrawPartition(BSPNode node) {
        // Draw a quad for the partioning, technically this isn't necessary once the rooms are populated
        // but useful to see how much space you have in a partition to draw rooms (and for debugging).
        Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        node.quadSpace = this.CreateQuad(this.partitions, "Quad " + node.name, 0.01f, node.bottomLeft, node.bottomRight, node.topLeft, node.topRight, randomColor);
    }

    public void DrawCorridors(BSPNode node, BSPNode node2) {
        // We want to draw corridors between two nodes, this method overcomes the challenge of 
        // deciding whether they are horizontal or verticle doors. To do this we can
        // find the two corner vectors which have the closest vectors of its siblings

        // Horizontal split
        float distance1 = Vector3.Distance(node.roomTopRight, node2.roomBottomRight);
        // Vertical split
        float distance2 = Vector3.Distance(node.roomBottomRight, node2.roomBottomLeft);

        List<Vector3> nodeExit = new List<Vector3>();
        List<Vector3> nodeExit2 = new List<Vector3>();
    
        Vector3 corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight;

        // Draw cubes to be used as walls.
        GameObject wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (distance1 < distance2) {
            // Vertical split (let's draw a corridor going left and right)
            // We want to  make sure the corridors connect where each room may be of varying sizes, so ensure its between the room edges.
            float minimumX = (node2.roomBottomLeft.x > node.roomTopLeft.x) ? node2.roomBottomLeft.x : node.roomTopLeft.x;
            float maximumX = (node2.roomBottomRight.x < node.roomTopRight.x) ? node2.roomBottomRight.x : node.roomTopRight.x;

            // X Position of corridor
            float xPosition = Random.Range(Mathf.RoundToInt(minimumX) + 1, Mathf.RoundToInt(maximumX - (this.corridorWidth + 1)));

            // Corners to draw quad
            corridorBottomLeft = new Vector3(xPosition, node.roomTopLeft.y, Mathf.RoundToInt(node.roomTopLeft.z));
            corridorBottomRight = new Vector3(xPosition + this.corridorWidth, node.roomTopRight.y, Mathf.RoundToInt(node.roomTopRight.z));
            corridorTopLeft = new Vector3(xPosition, node2.roomBottomLeft.y, Mathf.RoundToInt(node2.roomBottomLeft.z));
            corridorTopRight = new Vector3(xPosition + this.corridorWidth, node2.roomBottomRight.y, Mathf.RoundToInt(node2.roomBottomRight.z));

            nodeExit.Add(corridorBottomLeft);
            nodeExit.Add(corridorBottomRight);
            nodeExit2.Add(corridorTopLeft);
            nodeExit2.Add(corridorTopRight);

            // Draw walls
            wall1.transform.localPosition = ((corridorTopLeft + corridorBottomLeft) / 2) + (Vector3.left * 0.5f);
            wall2.transform.localPosition = ((corridorTopRight + corridorBottomRight) / 2) + (Vector3.right * 0.5f);
            wall1.transform.localScale += Vector3.forward * (this.corridorWidth + 1);
            wall2.transform.localScale += Vector3.forward * (this.corridorWidth + 1);

        } else {
            // Horizontal Split (let's draw a corridor going up and down)
            // We want to  make sure the corridors connect where each room may be of varying sizes, so ensure its between the room edges.
            float minimumY = (node2.roomBottomLeft.z > node.roomBottomRight.z) ? node2.roomBottomLeft.z : node.roomBottomRight.z;
            float maximumY = (node2.roomTopRight.z < node.roomTopLeft.z) ? node2.roomTopRight.z : node.roomTopLeft.z;

            // Z position of corridor
            float zPosition = Random.Range(Mathf.RoundToInt(minimumY) + 1, Mathf.RoundToInt(maximumY - (this.corridorWidth + 1)));
            corridorBottomLeft = new Vector3(Mathf.RoundToInt(node.roomBottomRight.x), node.roomBottomRight.y, zPosition);
            corridorBottomRight = new Vector3(Mathf.RoundToInt(node2.roomBottomLeft.x), node2.roomBottomLeft.y, zPosition);
            corridorTopLeft = new Vector3(Mathf.RoundToInt(node.roomTopRight.x), node.roomTopRight.y, zPosition + this.corridorWidth);
            corridorTopRight = new Vector3(Mathf.RoundToInt(node2.roomTopLeft.x), node2.roomTopLeft.y, zPosition + this.corridorWidth);

            nodeExit.Add(corridorBottomLeft);
            nodeExit.Add(corridorTopLeft);
            nodeExit2.Add(corridorBottomRight);
            nodeExit2.Add(corridorTopRight);

            // Draw walls
            wall1.transform.localPosition = (corridorTopLeft + corridorTopRight) / 2 + (Vector3.forward * 0.5f);
            wall2.transform.localPosition = (corridorBottomLeft + corridorBottomRight) / 2 + (Vector3.back * 0.5f);
            wall1.transform.localScale += Vector3.right * (this.corridorWidth + 1);
            wall2.transform.localScale += Vector3.right * (this.corridorWidth + 1);
        }

        // Record corridor vertices for automata later (we don't want to block it with a wall)
        node.corridorExits.Add(nodeExit);
        node2.corridorExits.Add(nodeExit2);

        string objectName = "Corridor " + node.name + " to " + node2.name; 
        GameObject corridor = this.CreateQuad(this.corridors, objectName, 0.4f, corridorBottomLeft, corridorBottomRight, corridorTopLeft, corridorTopRight, Settings.groundColor);
        
        // More settings for the walls
        wall1.transform.localScale += Vector3.up * this.wallHeight;
        wall2.transform.localScale += Vector3.up * this.wallHeight;
        wall1.GetComponent<Renderer>().material.color = Settings.wallColor;
        wall2.GetComponent<Renderer>().material.color = Settings.wallColor;
        wall1.transform.parent = corridor.transform;
        wall2.transform.parent = corridor.transform;
    }

    private GameObject CreateQuad(GameObject parentObj, string name, float elevation, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Color color)
    {
        // Create new quad object
        GameObject quad = new GameObject(name);
        // Set the quad object under the pcg parent (can be any game object)
        quad.transform.parent = parentObj.transform;
        // Rotate 90 degrees to fit in dungeon
        // quad.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
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