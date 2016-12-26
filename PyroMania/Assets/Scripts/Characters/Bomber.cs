using UnityEngine;
using System.Collections;

public class Bomber : MonoBehaviour {
    protected int XPos; // The X position of the bomber on the map
    protected int YPos; // The Y position of the bomber on the map
    protected int MaxBombs; // The Maximum amount of bombs that the bomber can place
    protected int MaxBombsBombCount; // The amount of bombs that the bomber has left
    protected int Lives; // The amount of lives that the bomber has left
    
    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        
    }
    
    // Sets the bomber position to the specified location on the map
    public void SetPosition(int XPos, int YPos) {
        this.XPos = XPos;
        this.YPos = YPos;
    }

    // Returns an array that has the location of the bomber on the map in this format [X, Y]
    public int[] GetPosition() {
        return new int[] {this.XPos, this.YPos};
    }

    // Sets the maximum amount of bombs that the bomber has to the specified amount
    public void SetMaxBombs(int MaxBombs) {
        this.MaxBombs = MaxBombs;
    }

    // Returns the maximum amount of bombs that the bomber has
    public int GetMaxBombs() {
        return this.MaxBombs;
    }

    // Sets the amount of lives that the bomber has to the specified amount
    public void SetLives(int Lives) {
        this.Lives = Lives;
    }

    // Returns the amount of lives that the bomber has
    public int GetLives() {
        return this.Lives;
    }

    // Hits the bomber - reduces his lives by 1, checks if he dies and triggers invulnerability effect
    public void GetHit() {
        //TODO
    }

    public void PlaceBomb() {
        Map.Instance.
    }

}
