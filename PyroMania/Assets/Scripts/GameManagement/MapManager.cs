using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour {

    protected GameObject[,] onlineMapInfo; // A 2d array containing all of the map's information
    protected GameObject[,] offlineMapInfo; // The tiles that changed offline but have not updated online yet
    protected List<GameObject> onlineBombs = new List<GameObject>(); // A list of all of the bombs
    protected List<GameObject> offlineBombs = new List<GameObject>(); // A list all of the offline bombs that have not updated online yet
    protected List<int[]> spawnLocs = new List<int[]>(); // A list of spawn locations on the map
    public GameObject groundTile; // A ground tile game object
    public GameObject bricksTile; // A bricks tile game object
    public GameObject wallTile; // A wall tile game object
    public GameObject missingTile; // A missing tile game object
    public GameObject bombTile; // A bomb tile game object
    public GameObject offlineBomber; // The offline bomber game object
    public GameObject[] onlineBombers; // The online bomber game objects

    // Loads the map from the file in the given location.
    public bool LoadMap(string mapData) {
        try {
            string[] newLine = { "\r\n" };
            string[] mapFileLines = mapData.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            string[] stringMapSize = mapFileLines[0].Split(' ');
            int[] mapSize = { int.Parse(stringMapSize[0]), int.Parse(stringMapSize[1]) };
            onlineMapInfo = new GameObject[mapSize[0], mapSize[1]];
            offlineMapInfo = new GameObject[mapSize[0], mapSize[1]];
            for (int Row = 0; Row < mapSize[0]; Row++) {
                string[] mapFileLine = mapFileLines[Row + 1].Split(' ');
                for (int Col = 0; Col < mapSize[1]; Col++) {
                    CreateTile(mapFileLine[Col], Col, mapSize[0] - 1 - Row, true);
                }
            }
            return true;
        } catch (IndexOutOfRangeException) {
            Debug.Log("Map data corrupt");
            return false;
        }
    }

    // Creates a tile game object from it's id and location
    public GameObject CreateTile(string tileId, int xPos, int yPos, bool online) {
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
            SetTile(xPos, yPos, tile, online);
        }
        return tile;
    }

    // Changes a specified tile on the map to the specified tile
    public void SetTile(int xPos, int yPos, GameObject tile, bool online) {
        if (online && !onlineMapInfo[yPos, xPos].GetComponent<Tile>().Equals(tile.GetComponent<Tile>())) {
            if (offlineMapInfo[yPos, xPos] != null) {
                Destroy(offlineMapInfo[yPos, xPos]);
            }
            offlineMapInfo[yPos, xPos] = null;
            if (onlineMapInfo[yPos, xPos] != null) {
                Destroy(onlineMapInfo[yPos, xPos]);
            }
            onlineMapInfo[yPos, xPos] = tile;
            tile.GetComponent<Tile>().SetLoc(xPos, yPos);
        } else if (!online && !offlineMapInfo[yPos, xPos].GetComponent<Tile>().Equals(tile.GetComponent<Tile>())) {
            if (offlineMapInfo[yPos, xPos] != null) {
                Destroy(offlineMapInfo[yPos, xPos]);
            }
            offlineMapInfo[yPos, xPos] = tile;
            tile.GetComponent<Tile>().SetLoc(xPos, yPos);
        }
    }

    // Adds a spawn location to the spawn location list
    protected void AddSpawnLoc(int[] spawnLocation) {
        if (spawnLocation.Length != 2) {
            Debug.Log("Bad spawn location array length");
        } else {
            spawnLocs.Add(spawnLocation);
        }
    }

    // Returns the tile at the given location
    public GameObject GetTile(int xPos, int yPos) {
        if (offlineMapInfo[xPos, yPos] != null)
            return offlineMapInfo[xPos, yPos];
        return onlineMapInfo[xPos, yPos];
    }

    // Creates a bomb
    public void CreateBomb(int xPos, int yPos, int creationTime, bool online) {
        GameObject newBomb = Instantiate(bombTile);
        newBomb.GetComponent<Bomb>().StartBomb(xPos, yPos, creationTime);
        GameObject oldBomb = GetBomb(xPos, yPos, online);
        if (oldBomb != null) {
            if (oldBomb.AddComponent<Bomb>().Equals(newBomb)) {
                Destroy(newBomb);
                return;
            }
            DestroyBomb(xPos, yPos, online);
        }
        List<GameObject> bombsList;
        if (online) {
            bombsList = onlineBombs;
            DestroyBomb(xPos, yPos, false);
        } else {
            bombsList = offlineBombs;
        }
        bombsList.Add(newBomb);
    }

    // Returns the bomb at the given location
    public GameObject GetBomb(int xPos, int yPos, bool online) {
        int[] location;
        List<GameObject> bombsList;
        if (online) {
            bombsList = onlineBombs;
        } else {
            bombsList = offlineBombs;
        }
        foreach (GameObject bomb in bombsList) {
            location = bomb.GetComponent<Bomb>().GetLocation();
            if (location[0] == xPos && location[1] == yPos) {
                return bomb;
            }
        }
        return null;
    }

    // Destroys the bomb and removes it from the map
    public void DestroyBomb(int xPos, int yPos, bool online) {
        List<GameObject> bombsList;
        if (online) {
            bombsList = onlineBombs;
        } else {
            bombsList = offlineBombs;
        }
        GameObject bomb = GetBomb(xPos, yPos, online);
        bombsList.Remove(bomb);
        Destroy(bomb);
    }

    // Returns all of the online or offline bombs
    public List<string> GetBombsInfo(bool online) {
        List<GameObject> bombsList;
        if (online) {
            bombsList = onlineBombs;
        } else {
            bombsList = offlineBombs;
        }
        List<string> bombsInfo = new List<string>();
        foreach(GameObject bomb in bombsList) {
            bombsInfo.Add(bomb.GetComponent<Bomb>().ToString());
        }
        return bombsInfo;
    }

    // Returns all of the bombers' info
    public List<string> GetBombersInfo() {
        List<string> bombersInfo = new List<string>();
        foreach (GameObject bomber in GetBombers()) {
            bombersInfo.Add(bomber.GetComponent<Bomber>().ToString());
        }
        return bombersInfo;
    }

    // Returns all of the bombers
    public GameObject[] GetBombers() {
        GameObject[] bombersArr = new GameObject[onlineBombers.Length + 1];
        Array.Copy(onlineBombers, bombersArr, onlineBombers.Length);
        onlineBombers[onlineBombers.Length - 1] = offlineBomber;
        return bombersArr;
    }

    // Returns the offline bomber
    public GameObject GetOfflineBomber() {
        return offlineBomber;
    }

    // Returns the online bomber array
    public GameObject[] GetOnlineBombers() {
        return onlineBombers;
    }

    // Returns the online bomber with the given name
    public GameObject GetOnlineBomber(string bomberName) {
        foreach (GameObject bomber in GetOnlineBombers()) {
            if (bomber.GetComponent<Bomber>().GetName() == bomberName) {
                return bomber;
            }
        }
        return null;
    }

    // Returns the locations where the tile of that type exists
    public List<int[]> GetTileLocs(Type type, bool online) {
        GameObject[,] mapInfo;
        if (online) {
            mapInfo = onlineMapInfo;
        } else {
            mapInfo = offlineMapInfo;
        }
        List<int[]> locsList = new List<int[]>();
        for (int i = 0; i < mapInfo.GetLength(0); i++) {
            for (int i2 = 0; i2 < mapInfo.GetLength(0); i2++) {
                if (mapInfo[i, i2] != null && mapInfo[i, i2].GetType() == type) {
                    locsList.Add(new int[] { i, i2 });
                }
            }
        }
        return locsList;
    }
}
