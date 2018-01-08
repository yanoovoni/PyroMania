using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
        if (Input.GetKey("up")) {

        }
        if (Input.GetKey("down")) {

        }
        if (Input.GetKey("right")) {

        }
        if (Input.GetKey("left")) {

        }
        if (Input.GetKey("space")) {
            GameManager.instance.mapManager.GetOfflineBomber().GetComponent<Bomber>().PlaceBomb();
        }
    }
}
