using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Detect a sensor object is being clicked.
public class ClickSelectScript : MonoBehaviour {
    // Sensors are on a certain layer mask.
    private int sensorLayerMask = 1 << 12;
    private UISlideScript uS;
    private Camera playerCam;
    private GameObject[] sensors;

    // Use this for initialization
    void Awake () {
        uS = GameObject.Find("UIPanel").GetComponent<UISlideScript>();
        playerCam = GetComponent<Camera>();
        sensors = GameObject.FindGameObjectsWithTag("Sensor");
    }
	
	// Update is called once per frame
	void Update () {
        if (!uS.panelVisible && Input.GetMouseButtonDown(0)) {
        // Use raycast, line out from camera forward, check for collisions on the sensor layer and display canvas and open sensor panel if applicable.
            RaycastHit hitInfo = new RaycastHit();

            bool hit = Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, sensorLayerMask);

            // If hit toggle the hit sensor and open the sensor panel
            if (hit) {
                if (!uS.panelVisible)
                    uS.TogglePanel();
                foreach (GameObject go in sensors) {
                    go.GetComponentInChildren<Toggle>().isOn = false;
                }
                hitInfo.transform.GetComponentInChildren<Toggle>().isOn = true;
            }
        }
    }
}

