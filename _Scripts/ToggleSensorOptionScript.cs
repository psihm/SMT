using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Sensor list options script
public class ToggleSensorOptionScript : MonoBehaviour {
    public SmtApiResponses.Sensor sensor;
    //public string sensorName;
    public GameObject list;
    public GameObject selectedListOpts;
    public GameObject listOpts;
    ListControlScript listCon;
    private GameObject copy;
    public GraphInfoScript gis;
    public GameObject api;

    // Use this for initialization
    void Awake () {
        list = GameObject.Find("SensorsList");
        listCon = list.GetComponent<ListControlScript>();
        listOpts = list.transform.Find("List/OptionsGrid").gameObject;
        selectedListOpts = GameObject.Find("SelectedList").transform.Find("List/OptionsGrid").gameObject;
        api = GameObject.Find("AnalyticsAPI");
        gis = GameObject.Find("GraphUI").GetComponent<GraphInfoScript>();
    }
	
    // Move list option from sensor list to selected and vice versa
	public void ToggleOption() {
        //sensorName = sensor.sensorName;
        bool isOn = GetComponent<Toggle>().isOn;
        if (gis == null) {
            Debug.Log("Graph Info Script is null!");
        }

        ScrollRect oldSR = transform.parent.parent.GetComponent<ScrollRect>();
        // If toggle on, and less than max types, add to selected sensors list.
        if (isOn) {
            // Check type by first word.
            if (gis.types.Count < gis.types.Capacity) {
                string s = sensor.sensorTypeName;
                string firstWord = s.IndexOf(" ") > -1
                  ? s.Substring(0, s.IndexOf(" "))
                  : s;

                if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                    firstWord = "Compression";
                }

                if (!gis.types.Contains(firstWord)) {
                    gis.types.Add(firstWord);
                }
                if (gis.types.Count >= gis.types.Capacity) {
                    listCon.DisableTypeToggles();
                }
            }
            gis.sensorstograph.Add(sensor);
            // Moves to selected list
            transform.SetParent(selectedListOpts.transform);
            if (gis.sensorstograph.Count >= gis.maxSensors) {
                listCon.DisableAll();
            }
        }

        // If toggle off, remove from selected list back to sensor list.
        else {
            gis.sensorstograph.RemoveAt(gis.sensorstograph.FindIndex(s => s.sensorID == sensor.sensorID));
            foreach (SmtApiResponses.Sensor s in gis.sensorstograph) {
            }
            transform.SetParent(listOpts.transform);
            // Clear types list and re-add types
            gis.types.Clear();
            foreach (SmtApiResponses.Sensor sen in gis.sensorstograph) {
                string s = sen.sensorTypeName;
                string firstWord = s.IndexOf(" ") > -1
                  ? s.Substring(0, s.IndexOf(" "))
                  : s;

                if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                    firstWord = "Compression";
                }

                if (!gis.types.Contains(firstWord)) {
                    gis.types.Add(firstWord);
                }
            }
            listCon.DisableTypeToggles();
        }

        oldSR.verticalNormalizedPosition = Mathf.Clamp(oldSR.verticalNormalizedPosition, 0f, 1f);
        transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = Mathf.Clamp(transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition, 0f, 1f);
        //listCon.transform.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = Mathf.Clamp(listCon.transform.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition, 0f, 1f);
        Canvas.ForceUpdateCanvases();
    }
}
