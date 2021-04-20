using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScript : MonoBehaviour {
    List<GameObject> sensors = new List<GameObject>(); // sensors touching the area collider bounds
    GameObject[] allSensors;
    AllSensorsScript allSS;

    // Use this for initialization
    void Awake () {
        allSS = GameObject.Find("Sensors").GetComponent<AllSensorsScript>();
        allSensors = GameObject.FindGameObjectsWithTag("Sensor");

        // If child of an area, reference parents sensors. Parents should find sensors within children's bounds.
        if (transform.parent.gameObject.tag == "Area") {
            sensors = transform.parent.GetComponent<AreaScript>().sensors;
        }

        // Find sensors within collider and within children colliders.
        else {
            foreach (GameObject go in allSensors) {
                foreach (Collider col in GetComponentsInChildren<Collider>()) {
                    if (go.GetComponent<Collider>() && col.bounds.Intersects(go.GetComponent<Collider>().bounds))
                        sensors.Add(go);
                }
            }
        }
	}

    void OnTriggerStay(Collider other) {
        foreach (GameObject go in sensors) {
            go.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (!allSS.allOn) {
            foreach (GameObject go in sensors) {
                go.SetActive(false);
            }
        }
    }
}
