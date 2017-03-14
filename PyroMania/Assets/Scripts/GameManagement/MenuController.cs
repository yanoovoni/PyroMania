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
        StartMultiplayer("127.0.0.1");
    }

    // Starts a multiplayer game using the given ip
    public void StartMultiplayer(string ip) {
        //TODO
    }

    // Starts a multiplayer game using the ip address from the input field in the menu
    public void StartMultiplayer() {
        //TODO
    }
}
