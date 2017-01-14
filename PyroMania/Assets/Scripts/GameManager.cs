using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public MapManager mapManager;

    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mapManager = GetComponent<MapManager>();
        } else if (instance != this)
            Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void LoadGame(string mapName) {
        CullingMaskLib camera = GameObject.Find("MainCamera").GetComponent<CullingMaskLib>();
        camera.HideAllLayers();
        camera.LayerCullingShow("UI");
        mapManager.LoadMap(mapName);
        camera.ShowAllLayers();
    }
}
