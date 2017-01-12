using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public MapManager mapManager;

    void Awake() {
        
    } 

    // Use this for initialization
    void Start () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        } else if (instance != this)
            Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
