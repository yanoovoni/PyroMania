using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public abstract class Tile : MonoBehaviour {
    protected const int spriteHeight = 1;
    protected const int spriteWidth = 1;
    protected int xPos;
    protected int yPos;

    // Use this for initialization
    void Start() {
    }

    public void SetLoc(int xPos, int yPos) {
        this.xPos = xPos;
        this.yPos = yPos;
        transform.position = new Vector2(xPos * spriteWidth, yPos * spriteHeight);
    }
}
