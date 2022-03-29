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
    // Grass Map
    private int[,] grassMap;
    // Fill Percent
    private int wallFillPercent;
    private int grassFillPercent;

    public CellularAutomata(Node node) {
        this.height = Mathf.RoundToInt(Vector3.Distance(node.roomTopRight, node.roomBottomRight)) + 1;
        this.width = Mathf.RoundToInt(Vector3.Distance(node.roomBottomLeft, node.roomBottomRight)) + 1;
        this.node = node;
        room = new int[width,height];
        grassMap = new int[width, height];
    }

    public void PopulateRoom(float wallHeight) {
        AssignFillPercent();
        CreateRoomNoise();
        OpenCorridors();
        FillRoomNoise();
        CreateGrassNoise();
        FillGrassNoise();
        DrawTiles(wallHeight);
    }

    public void AssignFillPercent() {
        float height = Vector3.Distance(node.roomTopRight, node.roomBottomRight);
        float width = Vector3.Distance(node.roomBottomLeft, node.roomBottomRight);
        this.wallFillPercent = Random.Range(25, 40);
        this.grassFillPercent = Random.Range(55, 65);

        if (width < 10.0f || height < 10.0f) {
            Debug.Log("Room " + node.name + " has a room length smaller than 10");
            this.wallFillPercent = Random.Range(10, 20);
        } else if(width < 6.0f || height < 6.0f) {
            Debug.Log("Room " + node.name + " has a room length smaller than 6");
            this.wallFillPercent = 0;
        }
    }

    private void CreateRoomNoise() {
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
                    room[x,y] = (pseudoRandom.Next(0,100) < wallFillPercent)? 1: 0;
                }
            }
        }
    }

    private void FillRoomNoise() {
		for (int i = 0; i < 5; i ++) {
			SmoothWalls();
		}
    }

	private void SmoothWalls() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = this.MooresNeighbourbood(x,y, room);

				if (neighbourWallTiles > 5) {
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
                    if(vertice1.z == 0) {
                        room[i, Mathf.RoundToInt(vertice1.z) + 1] = 0;
                    } else {
                        room[i, Mathf.RoundToInt(vertice1.z) - 1] = 0;
                    }
                }
            }
            else if (vertice1.x == vertice2.x) {
                for(int i= (int) vertice1.z; i < (int) vertice2.z; i++) {
                    try {
                        room[Mathf.RoundToInt(vertice1.x), i] = 0;
                    } catch (System.IndexOutOfRangeException e) {
                        if(vertice1.x != 0) {
                            Debug.Log("IndexOutOfRange Exception for room " + node.name);
                            Debug.Log("vertice1.x = " + vertice1.x + ". i = " + i);
                            room[Mathf.RoundToInt(vertice1.x) - 1, i] = 0;
                        }
                    }
                    if(vertice1.x == 0) {
                        room[Mathf.RoundToInt(vertice1.x) + 1, i] = 0;
                    } else {
                        room[Mathf.RoundToInt(vertice1.x) - 1, i] = 0;
                    }
                }
            }

        }
    }

	int MooresNeighbourbood(int gridX, int gridY, int[,] map) {
		int wallCount = 0;
        // Look in a three by three surrounding (Moores Neighborhood)
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
                // Check that the neighbouring position is not in the edges
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    // Don't include the tile itself that we're checking
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}
		return wallCount;
	}

    private void CreateGrassNoise() {
        // Random Seed
        string seed = Time.time.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        // Create some noise
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                if(room[x, y] == 0)
                    // Fill the rest of the area
                    grassMap[x,y] = (pseudoRandom.Next(0,100) < grassFillPercent)? 1: 0;
            }
        }
    }

    private void FillGrassNoise() {
		for (int i = 0; i < 5; i ++) {
            SmoothGrass();
		}
    }

    private void SmoothGrass() {
        int[,] tempGrassMap = new int[width,height];
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
                int neighbourGrassTiles = this.MooresNeighbourbood(x,y, grassMap);

                if (neighbourGrassTiles > 4) {
                    tempGrassMap[x,y] = 1;
                }
                else {
                    tempGrassMap[x,y] = 0;
                }
			}
		} 
        grassMap = tempGrassMap;
    }

    private int GetSurroundingGrassCount(int gridX, int gridY) {
        int grassCount = 0;
        // Look only up, down, left, right (Von Neumann Neighborhood)
        if(gridX - 1 >= 0 && gridX + 1 < width && gridY - 1 >= 0 && gridY + 1 < height) {
            grassCount += grassMap[gridX - 1, gridY];
            grassCount += grassMap[gridX + 1, gridY];
            grassCount += grassMap[gridX, gridY + 1];
            grassCount += grassMap[gridX, gridY - 1];
        }
        return grassCount;
    }

    public void DrawTiles(float wallHeight) {
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                Vector3 pos = new Vector3(x + .5f,0, y + .5f);
                Vector3 relativePosition = node.roomBottomLeft + pos;

                if(room[x, y] == 1) {
                    Tile tile = new Tile(node, "Wall", relativePosition, wallHeight);
                } else if(grassMap[x, y] == 1) {
                    Tile tile = new Tile(node, "Grass", relativePosition, wallHeight);
                } else {
                    Tile tile = new Tile(node, "Ground", relativePosition, wallHeight);
                }
            }
        }
    }
}
