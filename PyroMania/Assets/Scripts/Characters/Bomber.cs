using UnityEngine;
using System.Collections;

public class Bomber : MonoBehaviour {
    protected const float spriteHeight = 1;
    protected const float spriteWidth = 1;
    protected string bomberName; // The name of the bomber
    protected float xPos; // The X position of the bomber on the map
    protected float yPos; // The Y position of the bomber on the map
    protected int maxBombs; // The maximum amount of bombs that the bomber can place
    protected int bombCount; // The amount of bombs that the bomber has on the map
    protected int lives; // The amount of lives that the bomber has left

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    // Sets the bomber position to the specified location on the map
    public void SetPosition(float xPos, float yPos) {
        this.xPos = xPos;
        this.yPos = yPos;
        transform.position = new Vector2(xPos * spriteWidth + (float)(0.125), yPos * spriteHeight);
    }

    // Returns an array that has the location of the bomber on the map in this format [X, Y]
    public float[] GetPosition() {
        return new float[] { this.xPos, this.yPos };
    }

    // Sets the maximum amount of bombs that the bomber has to the specified amount
    public void SetMaxBombs(int maxBombs) {
        this.maxBombs = maxBombs;
    }

    // Returns the maximum amount of bombs that the bomber has
    public int GetMaxBombs() {
        return maxBombs;
    }

    // Sets the amount of lives that the bomber has to the specified amount
    public void SetLives(int lives) {
        this.lives = lives;
    }

    // Returns the amount of lives that the bomber has
    public int GetLives() {
        return lives;
    }

    // Hits the bomber - reduces his lives by 1, checks if he dies and triggers invulnerability effect
    public void GetHit() {
        //TODO
    }

    // Makes the bomber place a bomb at his location
    public void PlaceBomb() {
        //TODO
    }

    // Changes the bomber's info to the given BomberInfo's info
    public void SetInfo(BomberInfo bomberInfo) {
        name = bomberInfo.bomberName;
        SetPosition(bomberInfo.xPos, bomberInfo.yPos);
        maxBombs = bomberInfo.maxBombs;
        bombCount = 0;
        lives = bomberInfo.lives;
    }

    public class BomberInfo {
        public string bomberName; // The name of the bomber
        public float xPos; // The X position of the bomber on the map
        public float yPos; // The Y position of the bomber on the map
        public int maxBombs; // The maximum amount of bombs that the bomber can place
        public int lives; // The amount of lives the bomber has

        public BomberInfo(string bomberName, float xPos, float yPos, int lives) {
            this.bomberName = bomberName;
            this.xPos = xPos;
            this.yPos = yPos;
            maxBombs = 2;
            this.lives = lives;
        }

        public BomberInfo(string bomberName, float[] position, int lives) {
            this.bomberName = bomberName;
            xPos = position[0];
            yPos = position[1];
            maxBombs = 2;
            this.lives = lives;
        }
    }
}