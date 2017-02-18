using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    public class Bomb {
        protected int x;
        protected int y;
        protected int creationTime;
        protected Bomber bomber;
        protected Thread fuseThread;

        public Bomb(int x, int y, int creationTime, Bomber bomber) {
            this.x = x;
            this.y = y;
            this.creationTime = creationTime;
            this.bomber = bomber;
            bomber.PlaceBomb();
            fuseThread = new Thread(new ThreadStart(this.WaitForFuse));
            fuseThread.IsBackground = true;
            fuseThread.Start();
        }

        // Returns the location of the bomb
        public int[] GetLocation() {
            return new int[] { x, y };
        }

        // Returns a bomb back to the bomber and removes it from the map
        public void BlowUp() {
            bomber.ReturnBomb();
            MapManager.Instance.DeleteBomb(this);
        }

        // Waits for the bomb to explode and then calls BlowUp()
        protected void WaitForFuse() {
            Timer.Instance.Wait(creationTime, int.Parse(Settings.Instance.GetTempSetting("timer")));
            BlowUp();
        }

        public override string ToString() {
            return String.Format("{0},{1},{2}", x, y, creationTime);
        }
    }
}
