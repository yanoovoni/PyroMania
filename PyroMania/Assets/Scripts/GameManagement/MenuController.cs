using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update() {
    }

    // Starts a singleplayer game
    public void StartSingleplayer() {
        GameManager.instance.StartServer("");
        //TODO
    }

    // Starts a multiplayer game
    public void StartMultiplayer() {

    }
}
