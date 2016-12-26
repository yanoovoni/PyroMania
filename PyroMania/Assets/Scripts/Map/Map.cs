using UnityEngine;
using System;

public class Map : Singleton<Map> {
    protected Map() {} // guarantee this will be always a singleton only - can't use the constructor!
    protected int[,] MapInfo; // A 2d array containing all of the map's information
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
            this.MapInfo = new int[MapSize[0], MapSize[1]];
            for (int i = 0; i < MapSize[0]; i++) {
                string[] MapFileLine = MapFileLines[i + 1].Split(' ');
                for (int i2 = 0; i2 < MapSize[1]; i2++) {
                    this.MapInfo[i, i2] = int.Parse(MapFileLine[i2]);
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

    // Changes a specified tile on the map to the specified value
    public void SetTile(int XPos, int YPos, int Value) {
        this.MapInfo[XPos, YPos] = Value;
    }

    // Returns the value of a specified tile
    public int GetTile(int XPos, int YPos) {
        return this.MapInfo[XPos, YPos];
    }

    public void PlaceBomb(int XPos, int YPos) {
        //TODO
    }
}
