using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StartButton : MonoBehaviour {

    public class OnClick : UnityEvent {
        void invoke() {
            GameObject.Find("GameManager").GetComponent<GameManager>().LoadGame("Test");
        }
    }
}
