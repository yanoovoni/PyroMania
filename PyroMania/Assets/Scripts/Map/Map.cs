using UnityEngine;
using System;

public class Map : MonoBehaviour {
    public static Map instance = null; //Static instance of Map which allows it to be accessed by any other script.
    protected Map() {} // guarantee this will be always a singleton only - can't use the constructor!
    protected Tile[,] MapInfo; // A 2d array containing all of the map's information
    protected bool Visible; // Determines if the map is visible or not
    
    // Use this for initialization
    void Start () {
        this.Visible = false;
    }
    
    // Update is called once per frame
    void Update () {
        if (this.IsVisible()) {
            
        }
    }

    // Loads the map from the file in the given location.
    public bool LoadMap(string MapName) {
        try {
            string[] NewLine = { "\r\n" };
            TextAsset MapFile = Resources.Load("Maps/" + MapName) as TextAsset;
            if (MapFile == null) {
                print("Map file not found");
                return false;
            }
            string[] MapFileLines = MapFile.text.Split(NewLine, StringSplitOptions.RemoveEmptyEntries);
            string[] StringMapSize = MapFileLines[0].Split(' ');
            int[] MapSize = { int.Parse(StringMapSize[0]), int.Parse(StringMapSize[1]) };
            this.MapInfo = new Tile[MapSize[0], MapSize[1]];
            for (int i = 0; i < MapSize[0]; i++) {
                string[] MapFileLine = MapFileLines[i + 1].Split(' ');
                for (int i2 = 0; i2 < MapSize[1]; i2++) {
                    this.MapInfo[i, i2] = this.CreateTile(int.Parse(MapFileLine[i2]), i, i2);
                }
            }
            Resources.UnloadAsset(MapFile);
            return true;
        } catch (IndexOutOfRangeException) {
            Debug.Log("Map file corrupt");
            return false;
        }
    }

    // Sets the visibility of the map
    public void SetVisibility(bool Visible) {
        this.Visible = Visible;
    }

    // Returns if the map is visible
    public bool IsVisible() {
        return this.Visible;
    }

    // Creates a tile object from it's id and location
    private Tile CreateTile(int TileId, int XPos, int YPos) {
        Tile T = null;
        switch (TileId) {
            case 0:
                T = gameObject.AddComponent<Ground>();
                break;
            case 1:
                T = gameObject.AddComponent<Wall>();
                break;
            case 2:
                T = gameObject.AddComponent<Rock>();
                break;
        }
        if (T != null)
            T.SetLoc(XPos, YPos);
        return T;
    }

    // Changes a specified tile on the map to the specified value
    public void SetTile(int XPos, int YPos, Tile Value) {
        this.MapInfo[XPos, YPos] = Value;
    }

    // Returns the value of a specified tile
    public Tile GetTile(int XPos, int YPos) {
        return this.MapInfo[XPos, YPos];
    }

    public void PlaceBomb(int XPos, int YPos) {
        //TODO
    }
}
