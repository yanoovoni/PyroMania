using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;

public class MenuController : MonoBehaviour {
    public GameObject nameInputField;
    public GameObject ipInputField;
    public GameObject portInputField;
    public GameObject errorText;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update() {
    }

    // Starts a singleplayer game
    public void StartSingleplayer() {
        GameManager.instance.StartServer("");
        StartMultiplayer("Player", "127.0.0.1", 9000);
    }

    // Starts a multiplayer game using the given name, ip and port
    public void StartMultiplayer(string name, string ip, int port) {
        GameManager gameManager = GameManager.instance;
        gameManager.SetServerData(name, ip, port);
        gameManager.SetToGame();
    }

    // Starts a multiplayer game using the ip address from the input field in the menu
    public void StartMultiplayer() {
        string error = null;
        string name, ip;
        int port;
        name = GameObject.Find(nameInputField.name + "/Text").GetComponent<Text>().text;
        ip = GameObject.Find(ipInputField.name + "/Text").GetComponent<Text>().text;
        port = int.Parse(GameObject.Find(portInputField.name + "/Text").GetComponent<Text>().text);
        if (!ValidateName(name)) {
            error = "Invalid name";
        } else {
            if (!ValidateIPv4(ip)) {
                error = "Invalid ip address";
            } else {
                if (!ValidatePort(port)) {
                    error = "Invalid port";
                }
            }
        }
        if (error == null) {
            StartMultiplayer(name, ip, port);
        } else {
            errorText.GetComponent<Text>().text = error;
        }
    }

    // Validates a name
    private bool ValidateName(string name) {
        return !string.IsNullOrEmpty(name);
    }

    // Validates an ipv4 address
    private bool ValidateIPv4(string ipString) {
        if (string.IsNullOrEmpty(ipString)) {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4) {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    // Validates a port number
    private bool ValidatePort(int port) {
        return port > 1 && port < 65535;
    }
}
