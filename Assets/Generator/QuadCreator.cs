using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Taken from the unity3D Tutorial but modified to work with BSP
https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
*/
public class QuadCreator
{
    public GameObject createQuad(GameObject pcg, string name, float elevation, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Color color)
    {
        // Create new quad object
        GameObject quad = new GameObject(name);
        // Set the quad object under the pcg parent (can be any game object)
        quad.transform.parent = pcg.transform;
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