using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadMap : MonoBehaviour {

    // Use this for initialization
    void Start () {
        Map map = Map.Instance;
        map.LoadMap("Test");
        SceneManager.LoadSceneAsync("Game");
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
