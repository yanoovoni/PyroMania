using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour {

    protected Tile[,] mapInfo; // A 2d array containing all of the map's information

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    // Loads the map from the file in the given location.
    public bool LoadMap(string mapName) {
        try {
            string[] newLine = { "\r\n" };
            TextAsset mapFile = Resources.Load("Maps/" + mapName) as TextAsset;
            if (mapFile == null) {
                print("Map file not found");
                return false;
            }
            string[] mapFileLines = mapFile.text.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            string[] stringMapSize = mapFileLines[0].Split(' ');
            int[] mapSize = { int.Parse(stringMapSize[0]), int.Parse(stringMapSize[1]) };
            mapInfo = new Tile[mapSize[0], mapSize[1]];
            for (int i = 0; i < mapSize[0]; i++) {
                string[] mapFileLine = mapFileLines[i + 1].Split(' ');
                for (int i2 = 0; i2 < mapSize[1]; i2++) {
                    CreateTile(int.Parse(mapFileLine[i2]), i, i2);
                }
            }
            Resources.UnloadAsset(mapFile);
            return true;
        } catch (IndexOutOfRangeException) {
            Debug.Log("Map file corrupt");
            return false;
        }
    }

    // Creates a tile object from it's id and location
    protected Tile CreateTile(int tileId, int xPos, int yPos) {
        Tile tile = null;
        switch (tileId) {
            case 0:
                tile = gameObject.AddComponent<Ground>();
                break;
            case 1:
                tile = gameObject.AddComponent<Wall>();
                break;
            case 2:
                tile = gameObject.AddComponent<Rock>();
                break;
        }
        if (tile != null) {
            tile.SetLoc(xPos, yPos);
            SetTile(xPos, yPos, tile);
        }
        return tile;
    }

    // Changes a specified tile on the map to the specified tile
    public void SetTile(int xPos, int yPos, Tile tile) {
        mapInfo[xPos, yPos] = tile;
    }
}
