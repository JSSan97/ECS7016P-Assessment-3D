using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata
{
    // Node, record of all room information
    BSPNode node;
    // Room Width
    private int width;
    // Room Height
    private int height;
    // Map/Coordinates of room for building walls
    private int[,] wallMap;
    // Grass Map
    private int[,] grassMap;
    // Water Map
    private int[,] waterMap;

    // Fill Percent
    private int wallFillPercent;
    private int grassFillPercent;
    private int waterFillPercent;

    // Class to aid in the population of tiles in the room using Cellulalar Automata technique.
    public CellularAutomata(BSPNode node) {
        this.height = Mathf.RoundToInt(Vector3.Distance(node.roomTopRight, node.roomBottomRight)) + 1;
        this.width = Mathf.RoundToInt(Vector3.Distance(node.roomBottomLeft, node.roomBottomRight)) + 1;
        this.node = node;
        // Maps that tell us where to place certain tiles in the room.
        wallMap = new int[width,height];
        grassMap = new int[width, height];
        waterMap = new int[width, height];
    }

    public void PopulateRoom(float wallHeight) {
        // Assign how much tiles should be in a room
        AssignFillPercent();
        // Start with noise for the wall tiles
        CreateRoomNoise();
        // Open corridors so they are not closed by walls or water
        OpenCorridors();
        // Use the noise and cellular automata to smooth out the walls.
        FillWallNoise();
        // Now create noise for grass and water
        CreateGrassWaterNoise();
        // Use the noise and cellular automata to set the grass and water tiles
        FillGrassWaterNoise();
        // Open corridors so they are not closed
        OpenCorridors();
        // Draw tiles wrapper method
        DrawTiles(wallHeight);
    }

    public void AssignFillPercent() {
        // Set the expected noise for the wall, grass, water tiles
        this.wallFillPercent = Random.Range(30, 45);
        this.grassFillPercent = Random.Range(50, 65);
        this.waterFillPercent = Random.Range(50, 60);

        // We don't want too much cellular automata in narrow/small rooms.
        float height = Vector3.Distance(node.roomTopRight, node.roomBottomRight);
        float width = Vector3.Distance(node.roomBottomLeft, node.roomBottomRight);
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
					wallMap[x,y] = 1;
				} else {
                    // Fill the rest of the area
                    wallMap[x,y] = (pseudoRandom.Next(0,100) < wallFillPercent)? 1: 0;
                }
            }
        }
    }

    private void FillWallNoise() {
        // Smooth out walls 5 times.
		for (int i = 0; i < 5; i ++) {
			SmoothWalls();
		}
    }

	private void SmoothWalls() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = this.MooresNeighbourbood(x,y, wallMap);

				if (neighbourWallTiles > 5) {
					wallMap[x,y] = 1;
                    OpenCorridors();
                }
				else if (neighbourWallTiles < 4)
					wallMap[x,y] = 0;
			}
		}
	}

    private void OpenCorridors(){
        // We do not want the corridors to be blocked by wall tiles
        // So set the areas to the corridors to 0 in the map
        foreach(List<Vector3> corridor in node.corridorExits) {
            Vector3 vertice1 = corridor[0] - node.roomBottomLeft;
            Vector3 vertice2 = corridor[1] - node.roomBottomLeft;

            // Unblock corridors in the top and bottom sides of the room
            if(vertice1.z == vertice2.z) {
                for(int i = (int) vertice1.x; i < (int) vertice2.x; i++) {
                    try{
                        wallMap[i, Mathf.RoundToInt(vertice1.z)] = 0;
                        waterMap[i, Mathf.RoundToInt(vertice1.z)] = 0;
                    } catch (System.IndexOutOfRangeException e) {
                        Debug.Log("IndexOutOfRange Exception for room " + node.name);
                        Debug.Log("Corridor[0] = " + corridor[0]);
                        Debug.Log("Corridor[1] = " + corridor[1]);
                        Debug.Log("vertice1.z = " + vertice1.z + ". i = " + i);
                        continue;
                    }
                    if(vertice1.z == 0) {
                        wallMap[i, Mathf.RoundToInt(vertice1.z) + 1] = 0;
                        waterMap[i, Mathf.RoundToInt(vertice1.z) + 1] = 0;
                    } else {
                        wallMap[i, Mathf.RoundToInt(vertice1.z) - 1] = 0;
                        waterMap[i, Mathf.RoundToInt(vertice1.z) - 1] = 0;
                    }
                }
            }
            else if (vertice1.x == vertice2.x) {
                // Unblock corridors in the left and right sides of the room
                for(int i= (int) vertice1.z; i < (int) vertice2.z; i++) {
                    try {
                        wallMap[Mathf.RoundToInt(vertice1.x), i] = 0;
                        waterMap[Mathf.RoundToInt(vertice1.x), i] = 0;
                    } catch (System.IndexOutOfRangeException e) {
                        Debug.Log("IndexOutOfRange Exception for room " + node.name);
                        Debug.Log("Corridor[0] = " + corridor[0]);
                        Debug.Log("Corridor[1] = " + corridor[1]);
                        Debug.Log("vertice1.x = " + vertice1.x + ". i = " + i);
                        continue;
                    }
                    if(vertice1.x == 0) {
                        wallMap[Mathf.RoundToInt(vertice1.x) + 1, i] = 0;
                        waterMap[Mathf.RoundToInt(vertice1.x) + 1, i] = 0;
                    } else {
                        wallMap[Mathf.RoundToInt(vertice1.x) - 1, i] = 0;
                        waterMap[Mathf.RoundToInt(vertice1.x) - 1, i] = 0;
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

    private void CreateGrassWaterNoise() {
        // Random Seed
        string seed = Time.time.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        // Create some noise
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                if(this.wallMap[x, y] == 0) {
                    // Fill the rest of the area
                    this.grassMap[x,y] = (pseudoRandom.Next(0,100) < this.grassFillPercent)? 1: 0;

                    if(this.grassMap[x, y] == 0) {
                        this.waterMap[x, y] = (pseudoRandom.Next(0,100) < this.waterFillPercent)? 1: 0;
                    }
                }
            }
        }
    }

    private void FillGrassWaterNoise() {
        // Run five times to smooth out the grass and water
		for (int i = 0; i < 5; i ++) {
            this.grassMap = SmoothTiles(this.grassMap, 4);
            this.waterMap = SmoothTiles(this.waterMap, 3);
		}
    }

    private int[,] SmoothTiles(int[,] map, int rule) {
        // Smooth out the noise of grass an water tiles using moores neighbourhood.
        int[,] tempMap = new int[width,height];
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
                int neighbourTiles = this.MooresNeighbourbood(x,y, map);

                if (neighbourTiles > rule) {
                    tempMap[x,y] = 1;
                }
                else {
                    tempMap[x,y] = 0;
                }
			}
		} 
        return tempMap;
    }

    public void DrawTiles(float wallHeight) {
        // Now using the maps, start drawing out the tiles.
        for (int x=0; x < width; x++) {
            for (int y=0; y < height; y++) {
                Vector3 pos = new Vector3(x + .5f, 0, y + .5f);
                Vector3 relativePosition = node.roomBottomLeft + pos;

                if(wallMap[x, y] == 1) {
                    new Tile(node, "Wall", relativePosition, wallHeight);
                } else if(grassMap[x, y] == 1) {
                    new Tile(node, "Ground", relativePosition, wallHeight);
                    new Tile(node, "Grass", relativePosition, wallHeight);
                } else if(waterMap[x, y] == 1) {
                    new Tile(node, "Water", relativePosition, wallHeight);
                } else {
                    new Tile(node, "Ground", relativePosition, wallHeight);
                }
            }
        }
    }

    public int[,] getWallMap(){
        return this.wallMap;
    }
}
