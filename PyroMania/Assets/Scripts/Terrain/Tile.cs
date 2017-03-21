using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public abstract class Tile : MonoBehaviour {
    protected const float spriteHeight = 1;
    protected const float spriteWidth = 1;
    protected int xPos;
    protected int yPos;

    // Use this for initialization
    void Start() {
    }

    // Sets the location of the tile to the given value
    public void SetLoc(int xPos, int yPos) {
        this.xPos = xPos;
        this.yPos = yPos;
        transform.position = new Vector2(xPos * spriteWidth, yPos * spriteHeight);
    }

    // Returns the location of the tile
    public int[] GetLoc() {
        return new int[] { xPos, yPos };
    }

    // Checks if the object equals a given object
    public bool Equals(Tile tile) {
        int[] tileLoc = tile.GetLoc();
        return this.GetType() == tile.GetType() && this.xPos == tileLoc[0] && this.yPos == tileLoc[1];
    }
}
