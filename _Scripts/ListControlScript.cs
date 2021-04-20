using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Populate the sensor list with possible options
public class ListControlScript : MonoBehaviour {
    public bool isRunning;
    public GameObject listopts; // Assign in inspector, the sensor list options
    public GameObject apiobj;
    public List<string> types;
    public List<GameObject> sensorobjs;

    private GraphInfoScript gis;
    private Text titleText;
    // Use this for initialization
    public void Start() {
        isRunning = false;
        apiobj = GameObject.Find("AnalyticsAPI");
        gis = GameObject.Find("GraphUI").GetComponent<GraphInfoScript>();
        types = gis.types;
        sensorobjs = new List<GameObject>();
        titleText = GetComponentInChildren<Text>();
    }

    // Populate the list with togglable options
    public void PopulateList(string title, List<GameObject> sensors) {
        if (sensors == null || sensors.Count <= 0) {
            Debug.Log("Error populating list!");
        }
        sensorobjs.AddRange(sensors);
        titleText.text = title;

        // Hide types if two types already selected.
        foreach (GameObject s in sensorobjs) {
            string sen = s.GetComponent<ToggleSensorOptionScript>().sensor.sensorTypeName;
            string firstWord = sen.IndexOf(" ") > -1
              ? sen.Substring(0, sen.IndexOf(" "))
              : sen;

            if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                firstWord = "Compression";
            }

            //if (types.Count < types.Capacity || types.Contains(firstWord)) {
                s.SetActive(true);
            //}
        }
    }

    public void HideOptions() {
        foreach (Transform child in listopts.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void ClearOptions() {
        titleText.text = "Sensors List";
        HideOptions();
        sensorobjs = new List<GameObject>();
    }

    // Hide types outside of types list if types = 2
    public void HideExcludeTypes() {
        HideOptions();
        foreach (GameObject s in sensorobjs) {
            string sen = s.GetComponent<ToggleSensorOptionScript>().sensor.sensorTypeName;
            string firstWord = sen.IndexOf(" ") > -1
              ? sen.Substring(0, sen.IndexOf(" "))
              : sen;

            if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                firstWord = "Compression";
            }
            if (types.Count < types.Capacity || types.Contains(firstWord)) {//types.Contains(s.GetComponent<ToggleSensorOptionScript>().sensor.sensorTypeName)) {
                s.SetActive(true);
            }
        }
    }

    public void DisableTypeToggles() {
        foreach (Toggle tog in transform.GetComponentsInChildren<Toggle>(true)) {
            string s = tog.GetComponent<ToggleSensorOptionScript>().sensor.sensorTypeName;
            string firstWord = s.IndexOf(" ") > -1
              ? s.Substring(0, s.IndexOf(" "))
              : s;

            if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                firstWord = "Compression";
            }

            if (types.Contains(firstWord) || types.Count < types.Capacity) {
                tog.interactable = true;
            }
            else {
                tog.interactable = false;
            }
        }
    }

    public void DisableAll() {
        foreach (Toggle tog in transform.GetComponentsInChildren<Toggle>(true)) {
            tog.interactable = false;
        }
    }

    // Move list items back to sensor list
    public void RemoveItems() {
        while (listopts.transform.childCount > 0) {
            for (int i = 0; i < listopts.transform.childCount; i++) {
                GameObject child = listopts.transform.GetChild(i).gameObject;
                child.GetComponent<Toggle>().isOn = !child.gameObject.GetComponent<Toggle>().isOn;
                child.GetComponent<ToggleSensorOptionScript>().ToggleOption();
            }
        }
    }
}
