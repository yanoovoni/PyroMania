using UnityEngine;
using System.Collections;

public class MapLoader : MonoBehaviour {

    // Use this for initialization
    void Start () {
        GameManager gameManager = GameManager.instance;
        gameManager.mapManager.LoadMap("Test");
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
