using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSensorsScript : MonoBehaviour {
    public GameObject[] sensors;
    public bool allOn = false;
	// Use this for initialization
	void Awake () {
        sensors = GameObject.FindGameObjectsWithTag("Sensor");
        if (sensors == null) {
            Debug.Log("Sensors are null!");
        }
	}

    void Start () {
        foreach (GameObject go in sensors) {
            go.SetActive(false);
        }
    }

    // Toggle on all sensors
    public void ToggleAll() {
        allOn = !allOn;
        foreach (GameObject go in sensors) {
            go.SetActive(allOn);
        }
    }
}
