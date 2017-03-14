using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class Timer : MonoBehaviour {
    protected Stopwatch stopWatch;

    // Use this for initialization
    void Start() {
        stopWatch = new Stopwatch();
    }

    // Update is called once per frame
    void Update() {

    }

    // Starts the clock
    public void StartTimer() {
        stopWatch.Start();
    }

    // Returns for how long the timer was running
    public long GetTime() {
        return stopWatch.ElapsedMilliseconds;
    }

    // Waits the given time
    public void Wait(int time) {
        Thread.Sleep(time);
    }

    // Waits the given duration as if it was started at the given start time
    public void Wait(long startTime, long duration) {
        int waitTime = (int)(duration - stopWatch.ElapsedMilliseconds - startTime);
        if (waitTime > 0)
            Thread.Sleep(waitTime);
    }
}
