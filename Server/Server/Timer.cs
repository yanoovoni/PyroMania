using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server {
    public class Timer {
        protected static Timer instance; // The instance of the singleton
        protected Stopwatch stopWatch;

        protected Timer() {

        }

        // Returns the instance of the singleton, creates a new one if there isn't one
        public static Timer Instance {
            get {
                if (instance == null) {
                    instance = new Timer();
                }
                return instance;
            }
        }

        // Starts the clock
        public void Start() {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        // Returns for how long the timer was running
        public int GetTime() {
            return (int)stopWatch.ElapsedMilliseconds;
        }

        // Waits the given time
        public void Wait(int time) {
            Thread.Sleep(time);
        }

        // Waits the given duration as if it was started at the given start time
        public void Wait(int startTime, int duration) {
            int waitTime = duration - ((int)stopWatch.ElapsedMilliseconds - startTime);
            if (waitTime > 0)
                Thread.Sleep(waitTime);
        }
    }
}
