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

        public Bomber(string name, double x = 0, double y = 0) {
            this.name = name;
            this.x = x;
            this.y = y;
            this.health = int.Parse(Settings.Instance.GetTempSetting("health"));
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

        // Returns how much health the bomber has
        public int GetHealth() {
            return health;
        }

        public override string ToString() {
            return String.Format("{0},{1},{2},{3}", name, x, y, health);
        }
    }
}
