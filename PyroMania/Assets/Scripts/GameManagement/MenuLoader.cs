using UnityEngine;
using System.Collections;

public class MenuLoader : MonoBehaviour {

    public GameObject gameManager; //GameManager prefab to instantiate.

    // Use this for initialization
    void Awake() {
        if (GameManager.instance == null) //Check if a GameManager has already been assigned to static variable GameManager instance or if it's still null
            Instantiate(gameManager); //Instantiate gameManager prefab
    }

    // Update is called once per frame
    void Update () {
    }
}
