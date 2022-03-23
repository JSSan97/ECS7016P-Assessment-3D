using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata
{
    // Node, record of all room information
    Node node;
    // Room Width
    private int width;
    // Room Height
    private int height;
    // Coordinates of room
    private int[,] room;
    // Fill Percent
    private int fillPercent;

    public CellularAutomata(Node node) {
        this.height = Mathf.RoundToInt(Vector3.Distance(node.roomTopRight, node.roomBottomRight)) + 1;
        this.width = Mathf.RoundToInt(Vector3.Distance(node.roomBottomLeft, node.roomBottomRight)) + 1;
        this.fillPercent = Random.Range(20, 40);
        this.node = node;
        room = new int[width,height];
    }

    public void PopulateRoom(float wallHeight) {
        CreateNoise();
        OpenCorridors();
        FillNoise();
        DrawRoom(wallHeight);
    }

    private void CreateNoise() {
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
                    room[x,y] = (pseudoRandom.Next(0,100) < fillPercent)? 1: 0;
                }
            }
        }
    }

    private void FillNoise() {
		for (int i = 0; i < 5; i ++) {
			SmoothWalls();
		}
    }

	private void SmoothWalls() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = this.GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4) {
					room[x,y] = 1;
                    OpenCorridors();
                }
				else if (neighbourWallTiles < 4)
					room[x,y] = 0;
			}
		}
	}

    private void OpenCorridors(){
        foreach(List<Vector3> corridor in node.corridorExits) {
            Vector3 vertice1 = corridor[0] - node.roomBottomLeft;
            Vector3 vertice2 = corridor[1] - node.roomBottomLeft;

            if(vertice1.z == vertice2.z) {
                for(int i = (int) vertice1.x; i < (int) vertice2.x; i++) {
                    room[i, Mathf.RoundToInt(vertice1.z)] = 0;
                }
            }
            else if (vertice1.x == vertice2.x) {
                for(int i= (int) vertice1.z; i < (int) vertice2.z; i++) {
                    room[Mathf.RoundToInt(vertice1.x), i] = 0;
                }
            }

        }
    }

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
        // Look in a three by three square
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += room[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}
		return wallCount;
	}

    public void DrawRoom(float wallHeight) {
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = this.node.quadRoom.transform;
                if(room[x, y] == 1)
                    cube.transform.localScale += Vector3.up * wallHeight;
                Vector3 pos = new Vector3(x + .5f,0, y + .5f);
                cube.transform.localPosition = node.roomBottomLeft + pos;
                cube.GetComponent<Renderer>().material.color = (room[x, y] == 1) ? Color.black : Color.white;
            }
        }
    }
}
