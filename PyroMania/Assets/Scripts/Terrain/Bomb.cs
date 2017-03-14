﻿using UnityEngine;
using System.Collections;
using System.Threading;

public class Bomb : Tile {
    protected int creationTime;
    protected Thread fuseThread;

    public void StartBomb(int x, int y, int creationTime) {
        SetLoc(x, y);
        this.creationTime = creationTime;
        fuseThread = new Thread(new ThreadStart(this.WaitForFuse));
        fuseThread.IsBackground = true;
        fuseThread.Start();
    }

    // Returns the location of the bomb
    public int[] GetLocation() {
        return new int[] { xPos, yPos };
    }

    // Returns a bomb back to the bomber and removes it from the map
    public void BlowUp() {
        //TODO
    }

    // Waits for the bomb to explode and then calls BlowUp()
    protected void WaitForFuse() {
        GameManager.instance.timer.Wait(creationTime, 3000);
        BlowUp();
    }

    public override string ToString() {
        return System.String.Format("{0},{1},{2}", xPos, yPos, creationTime);
    }
}
