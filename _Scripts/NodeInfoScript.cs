using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Get the list of sensors per node.
public class NodeInfoScript : MonoBehaviour {
    private GameObject list;
    private GameObject listOpts;
    private GameObject analyticsAPI;
    private UISlideScript uS;
    private List<SmtApiResponses.Sensor> SmtApiResponses;
    private SmtApiScript api;

    public Material mat;
    public List<GameObject> sensors;

    // Assign these in Inspector
    public string nodeID; 
    public string nodeName;
    public GameObject listItemPrefab;
    public Material selectedMat;

    // Use this for initialization
    void Start () {
        mat = GetComponent<Renderer>().material;
        list = GameObject.Find("SensorsList");
        listOpts = list.transform.Find("List/OptionsGrid").gameObject;
        analyticsAPI = GameObject.Find("AnalyticsAPI");
        api = gameObject.AddComponent<SmtApiScript>();
        sensors = new List<GameObject>();
        api.headers = analyticsAPI.GetComponent<SmtApiScript>().headers;
        api.errorBox = analyticsAPI.GetComponent<SmtApiScript>().errorBox;
        uS = GameObject.Find("UIPanel").GetComponent<UISlideScript>();
        NewSensorList();
    }
    
    // Call when node is toggled. Pull panel out and change color on selection, revert back when toggled off
    public void ToggleSensorList() {
        Toggle t = GetComponent<Toggle>();
        if (!uS.panelVisible) {
            uS.TogglePanel();
        }
        if (t.isOn) {
            GetSensorList();
            GetComponent<Renderer>().material = selectedMat;
        }
        else {
            GetComponent<Renderer>().material = mat;
        }
    }

    // Load the sensor list with this nodes sensors
    public void GetSensorList() {
        list.GetComponent<ListControlScript>().ClearOptions();
        list.GetComponent<ListControlScript>().PopulateList(nodeName, sensors);
    }

    //public void AddSensorsToList() {
    //    if (GetComponent<Toggle>().isOn) {
    //        foreach(NodeInfoScript nis in GetComponents<NodeInfoScript>()) {
    //            list.GetComponent<ListControlScript>().PopulateList(nis.nodeName, nis.sensors);
    //        }
    //    }
    //}

    public void NewSensorList() {
        api.LoadSensorList(nodeID);
        SmtApiResponses = new List<SmtApiResponses.Sensor>(api.sensors);
        if (api.headers == null) {            
            api.headers = analyticsAPI.GetComponent<SmtApiScript>().headers;
            api.LoadSensorList(nodeID);
            SmtApiResponses = new List<SmtApiResponses.Sensor>(api.sensors);
            if (api.headers == null) {
                StartCoroutine(api.ReportError("Coud not retrieve node!"));
                return;
            }
        }
        
        foreach (SmtApiResponses.Sensor s in SmtApiResponses) {
            string lowername = s.sensorName.ToLower();
            string lowertype = s.sensorTypeName.ToLower();
            if (!lowername.Contains("new") && !lowertype.Contains("unknown") || lowername.Contains("string")) {
                GameObject li = Instantiate(listItemPrefab, listOpts.transform);
                li.GetComponent<ToggleSensorOptionScript>().sensor = s;
                Text litext = li.GetComponentInChildren<Text>();
                litext.text = s.sensorName + "\nType: " + s.sensorTypeName;
                //Debug.Log("Added sensorid: " + s.sensorID);
                if (lowername.Contains("string")) {
                    litext.text = s.sensorName + "\nType: Compression";
                }
                li.SetActive(false);
                li.GetComponent<Toggle>().interactable = true;                
                sensors.Add(li);
            }
        }
    }
}
