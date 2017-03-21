using UnityEngine;
using System.Collections;

public class Bricks : Tile {
    public bool destroyed = false;

    public void BlowUp(bool online) {
        GameManager.instance.mapManager.GetComponent<MapManager>().CreateTile("0", xPos, yPos, online);
    }
}
