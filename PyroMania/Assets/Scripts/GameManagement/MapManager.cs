using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour {

    protected GameObject[,] mapInfo; // A 2d array containing all of the map's information
    protected List<int[]> spawnLocs = new List<int[]>(); // A list of spawn locations on the map
    public GameObject groundTile; // A ground tile game object
    public GameObject bricksTile; // A bricks tile game object
    public GameObject wallTile; // A wall tile game object
    public GameObject missingTile; // A missing tile game object
    public GameObject offlineBomber; // The offline bomber game object
    public GameObject[] onlineBombers; // The online bomber game objects

    // Loads the map from the file in the given location.
    public bool LoadMap(string mapData) {
        try {
            string[] newLine = { "\r\n" };
            string[] mapFileLines = mapData.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            string[] stringMapSize = mapFileLines[0].Split(' ');
            int[] mapSize = { int.Parse(stringMapSize[0]), int.Parse(stringMapSize[1]) };
            mapInfo = new GameObject[mapSize[0], mapSize[1]];
            for (int Row = 0; Row < mapSize[0]; Row++) {
                string[] mapFileLine = mapFileLines[Row + 1].Split(' ');
                for (int Col = 0; Col < mapSize[1]; Col++) {
                    CreateTile(mapFileLine[Col], Col, mapSize[0] - 1 - Row);
                }
            }
            return true;
        } catch (IndexOutOfRangeException) {
            Debug.Log("Map data corrupt");
            return false;
        }
    }

    // Creates a tile game object from it's id and location
    protected GameObject CreateTile(string tileId, int xPos, int yPos) {
        GameObject tile = null;
        switch (tileId) {
            case "0": // Ground tile
            case "3": // Spawn tile
                tile = groundTile;
                if (tileId == "3") {
                    AddSpawnLoc(new int[]{xPos, yPos});
                }
                break;
            case "1": // Wall tile
                tile = wallTile;
                break;
            case "2": // Bricks tile
                tile = bricksTile;
                break;
            default: // Missing tile
                tile = missingTile;
                break;
        }
        if (tile != null) {
            tile = Instantiate(tile);
            SetTile(xPos, yPos, tile);
        }
        return tile;
    }

    // Changes a specified tile on the map to the specified tile
    public void SetTile(int xPos, int yPos, GameObject tile) {
        if (mapInfo[yPos, xPos] != null) {
            Destroy(mapInfo[yPos, xPos]);
        }
        mapInfo[yPos, xPos] = tile;
        tile.GetComponent<Tile>().SetLoc(xPos, yPos);
    }

    // Adds a spawn location to the spawn location list
    protected void AddSpawnLoc(int[] spawnLocation) {
        if (spawnLocation.Length != 2) {
            Debug.Log("Bad spawn location array length");
        } else {
            spawnLocs.Add(spawnLocation);
        }
    }
}
