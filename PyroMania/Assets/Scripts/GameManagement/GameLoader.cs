using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour {

    void Start() {
        GameManager.instance.LoadGame();
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
