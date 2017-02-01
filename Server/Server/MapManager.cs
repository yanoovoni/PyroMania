using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class MapManager {
        protected static MapManager instance; // The instance of the singleton
        protected string[,] mapInfo; // A 2d array containing all of the map's information
        protected List<int[]> spawnLocs = new List<int[]>(); // A list of spawn locations on the map 

        protected MapManager() { }

        // Returns the instance of the singleton, creates a new one if there isn't one
        public static MapManager Instance {
            get {
                if (instance == null) {
                    instance = new MapManager();
                }
                return instance;
            }
        }

        // Loads the map from the file in the given location.
        public bool LoadMap(string mapName) {
            try {
                string[] newLine = { "\r\n" };
                StreamReader mapFile = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\Files\Maps\" + mapName + ".pmm");
                if (mapFile == null) {
                    Printer.Print("Map file not found");
                    return false;
                }
                string[] mapFileLines = mapFile.ReadToEnd().Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                string[] stringMapSize = mapFileLines[0].Split(' ');
                int[] mapSize = { int.Parse(stringMapSize[0]), int.Parse(stringMapSize[1]) };
                mapInfo = new string[mapSize[0], mapSize[1]];
                for (int Row = 0; Row < mapSize[0]; Row++) {
                    string[] mapFileLine = mapFileLines[Row + 1].Split(' ');
                    for (int Col = 0; Col < mapSize[1]; Col++) {
                        SetTile(Col, mapSize[0] - 1 - Row, mapFileLine[Col]);
                    }
                }
                return true;
            } catch (IndexOutOfRangeException) {
                Printer.Print("Map file corrupt");
                return false;
            }
        }

        // Changes a specified tile on the map to the specified tile
        public void SetTile(int xPos, int yPos, string tile) {
            mapInfo[yPos, xPos] = tile;
            if (tile == "3") {
                AddSpawnLoc(new int[]{xPos, yPos});
            }
        }

        // Adds a spawn location to the spawn location list
        protected void AddSpawnLoc(int[] spawnLocation) {
            if (spawnLocation.Length != 2) {
                Printer.Print("Bad spawn location array length");
            } else {
                spawnLocs.Add(spawnLocation);
            }
        }
    }

}
