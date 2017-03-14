using UnityEngine;
using System.Collections;

public class Bricks : Tile {
    public void BlowUp() {
        GameManager.instance.mapManager.GetComponent<MapManager>().CreateTile("0", xPos, yPos);
    }
}
