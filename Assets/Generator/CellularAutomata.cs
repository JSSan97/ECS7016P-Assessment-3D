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
        this.fillPercent = Random.Range(0, 30);
        this.node = node;
        room = new int[width,height];
    }

    public void populateRoom() {
        createNoise();
        fillNoise();
        drawRoom();
    }

    public void createNoise() {
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

        // Replace corridors ends with zeroes
        openCorridors();
    }

    void fillNoise() {
		for (int i = 0; i < 5; i ++) {
			smoothRoom();
		}
    }

	void smoothRoom() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = this.getSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4) {
					room[x,y] = 1;
                    openCorridors();
                }
				else if (neighbourWallTiles < 4)
					room[x,y] = 0;
			}
		}
	}

    private void openCorridors(){
        // Replace corridors ends with zeroes
        foreach(List<Vector3> corridor in node.corridorExits) {
            Vector3 vertice1 = corridor[0] - node.roomBottomLeft;
            Vector3 vertice2 = corridor[1] - node.roomBottomLeft;

            if (vertice1.z == vertice2.z) {
                for(int i=Mathf.RoundToInt(vertice1.x); i < Mathf.RoundToInt(vertice2.x) ; i++) {
                    room[i, Mathf.RoundToInt(vertice1.z)] = 0;
                }
            } else if (vertice1.x == vertice2.x) {
                for(int i=Mathf.RoundToInt(vertice1.z); i < Mathf.RoundToInt(vertice2.z); i++) {
                    room[Mathf.RoundToInt(vertice1.x), i] = 0;
                }
            }
        }
    }

	int getSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
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

    public void drawRoom() {
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = this.node.quadRoom.transform;
                float centreX = (node.roomBottomLeft.x + node.roomBottomRight.x) / 2;
                float centreZ = (node.roomTopRight.z + node.roomBottomRight.z) / 2;
                Vector3 pos1 = new Vector3(centreX, 0, centreZ);
				Vector3 pos2 = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
                
                cube.transform.localPosition = pos1 + pos2;
                cube.GetComponent<Renderer>().material.color = (room[x, y] == 1) ? Color.black : Color.white;
            }
        }
    }
}
