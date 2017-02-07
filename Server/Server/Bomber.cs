using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class Bomber {
        private string name; // The name of the bomber
        private double x; // The x location of the bomber
        private double y; // The y location of the bomber
        private int health; // The amount of health the bomber has left
        private int bombs; // The amount of bombs the bomber has left. Can't be lower than 0
        
        public Bomber(string name, double x, double y) {
            this.name = name;
            this.x = x;
            this.y = y;
            this.health = int.Parse(Settings.Instance.GetTempSetting("health"));
            this.bombs = int.Parse(Settings.Instance.GetTempSetting("bombs"));
        }

        // Returns the name of the bomber
        public string GetName() {
            return name;
        }

        // Sets a given position to the bomber
        public void SetPosition(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public void SetPosition(double[] position) {
            SetPosition(position[0], position[1]);
        }

        // Returns the position of the bomber
        public double[] GetPosition() {
            return new double[] { x, y };
        }

        // Makes the player lose 1 health and returns true if he is still alive
        public bool GetHit() {
            health--;
            return health > 0;
        }

        // Returns how many bombs the bomber can place
        public int BombsLeft() {
            return bombs;
        }

        // Reduces the bomb count by one if possible and returns if it was successful
        public bool PlaceBomb() {
            if (bombs > 0) {
                bombs--;
                return true;
            }
            return false;
        }

        // Increases the bomb count by one
        public void ReturnBomb() {
            bombs++;
        }
    }
}
