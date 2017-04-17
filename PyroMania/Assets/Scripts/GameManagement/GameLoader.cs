using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

    // Use this for initialization
    void Start () {
        GameManager.instance.LoadGame();
    }

    // Update is called once per frame
    void Update () {

    }
}
