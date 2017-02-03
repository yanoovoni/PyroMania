using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null; // The instance of the game manager
    public MapManager mapManager; // The map manager
    public GameObject controller; // The current controller
    public GameObject[] controllers; // The controllers
    public static int menuControllerIndex = 0; // The index of the menu controller
    public static int gameControllerIndex = 1; // The index of the game controller


    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mapManager = GetComponent<MapManager>();
            SetToMenu();
        } else if (instance != this)
            Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update () {

    }

    // Loads the game
    public void LoadGame() {
        //TODO
        CullingMaskLib camera = GameObject.Find("MainCamera").GetComponent<CullingMaskLib>();
        camera.HideAllLayers();
        camera.LayerCullingShow("UI");
        
        mapManager.LoadMap();
        camera.ShowAllLayers();
    }

    // Starts the game scene
    public void SetToGame() {
        if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Game"))) {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        controller = Instantiate(controllers[gameControllerIndex]);
    }

    // Starts the menu scene
    public void SetToMenu() {
        if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Menu"))) {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        controller = Instantiate(controllers[menuControllerIndex]);
    }

    // Starts a server
    public void StartServer(string args) {
        Process serverProcess = new Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"\Server\Server.exe";
        startInfo.CreateNoWindow = true;
        startInfo.Arguments = args;
        serverProcess.StartInfo = startInfo;
        serverProcess.Start();
    }
}
