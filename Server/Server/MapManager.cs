using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class MapManager {
        protected static MapManager instance; // The instance of the singleton
        protected string mapData; // A 2d array containing all of the map's information
        protected List<int[]> spawnLocs = new List<int[]>(); // The list of spawn locations on the map
        protected List<int[]> rockLocs = new List<int[]>(); // The list of rock locations on the map
        protected List<Bomber> bombers = new List<Bomber>(); // The list of bombers in the game
        protected List<Bomb> bombs = new List<Bomb>(); // The list of bombs on the map

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
                mapData = mapFile.ReadToEnd();
                string[] mapFileLines = mapData.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                string[] stringMapSize = mapFileLines[0].Split(' ');
                int[] mapSize = { int.Parse(stringMapSize[0]), int.Parse(stringMapSize[1]) };
                for (int Row = 0; Row < mapSize[0]; Row++) {
                    string[] mapFileLine = mapFileLines[Row + 1].Split(' ');
                    for (int Col = 0; Col < mapSize[1]; Col++) {
                        AddTile(Col, mapSize[0] - 1 - Row, mapFileLine[Col]);
                    }
                }
                return true;
            } catch (IndexOutOfRangeException) {
                Printer.Print("Map file corrupt");
                return false;
            }
        }

        // Changes a specified tile on the map to the specified tile
        public void AddTile(int xPos, int yPos, string tile) {
            switch (tile) {
                case "2": // If the tile is a rock
                    AddRockLoc(new int[] { xPos, yPos });
                    break;
                case "3": // If the tile is a spawn position
                    AddSpawnLoc(new int[] { xPos, yPos });
                    break;
            }
        }

        // Adds a spawn location to the spawn locations list
        protected void AddSpawnLoc(int[] spawnLocation) {
            if (spawnLocation.Length != 2) {
                Printer.Print("Bad spawn location array length");
            } else {
                spawnLocs.Add(spawnLocation);
            }
        }

        // Adds a rock location to the rock locations list
        public void AddRockLoc(int[] rockLocation) {
            if (rockLocation.Length != 2) {
                Printer.Print("Bad spawn location array length");
            } else {
               rockLocs.Add(rockLocation);
            }
        }

        // Deletes a the rock at the given location
        public void DeleteRock(int[] rockLocation) {
            if (rockLocation.Length != 2) {
                Printer.Print("Bad rock location array length");
            } else {
                rockLocs.Remove(rockLocation);
            }
        }

        // Returns the bomber with the given name. Returns null if the bomber does not exist
        public Bomber GetBomber(string name) {
            for (int i = 0; i < bombers.Count; i++) {
                Bomber b = bombers.ElementAt(i);
                if (b.GetName() == name) {
                    return b;
                }
            }
            return null;
        }

        // Adds a bomber to the bomber list
        public void AddBomber(Bomber bomber) {
            bombers.Add(bomber);
        }

        // Adds a bomb to the bomb list
        public void AddBomb(Bomb bomb) {
            bombs.Add(bomb);
        }

        // Deletes the bomb from the bomb list and returns true if the bomb was deleted
        public bool DeleteBomb(Bomb bomb) {
            return bombs.Remove(bomb);
        }

        // Deleted the bomb in the given location and returns true if the bomb was deleted
        public bool DeleteBomb(int[] bombLoc) {
            for (int i = 0; i < bombs.Count; i++) {
                int[] curBombLoc = bombs.ElementAt(i).GetLocation();
                if (curBombLoc[0] == bombLoc[0] && curBombLoc[1] == bombLoc[1]) {
                    bombs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        // Returns how many slots are left in the server
        public int SlotsLeft() {
            return spawnLocs.Count - bombers.Count;
        }

        // Returns the map's data
        public string GetMap() {
            return mapData;
        }

        // Returns the rocks locations list
        public List<int[]> GetRockLocs() {
            return rockLocs;
        }

        // Returns the bombers list
        public List<Bomber> GetBombers() {
            return bombers;
        }

        // Returns the bombs list
        public List<Bomb> GetBombs() {
            return bombs;
        }

        // Spawns the bombers
        public void SpawnBombers() {
            for(int i = 0; i < bombers.Count; i++) {
                bombers.ElementAt(i).SetPosition(new float[] { spawnLocs.ElementAt(i)[0], spawnLocs.ElementAt(i)[1] });
            }
        }
    }
}
