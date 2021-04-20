using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorInfoScript : MonoBehaviour {

    public float refreshTime = 3600f; //Seconds
    public string sensorid;
    public Material selectedMat;
    public Material dangerMat;
    public SmtApiResponses.Reading reading;

    protected GameObject mapList;
    protected Material mat;
    protected GameObject listOpts;
    protected GameObject list;
    protected GameObject[] nodes;
    protected GameObject uiParent;
    protected MapSlideScript mapPanelToggle;
    protected UISlideScript uS;
    protected ToggleSensorOptionScript tsos;
    protected SmtApiScript sas;
    protected Renderer rend;

    // Use this for initialization
    void Awake () {
        rend = GetComponent<Renderer>();
        if (rend == null) {
            rend = transform.parent.GetComponent<Renderer>();
        }
        mat = rend.material;
        list = GameObject.Find("SensorsList").gameObject;
        listOpts = list.transform.Find("List").Find("OptionsGrid").gameObject;
        uS = GameObject.Find("UIPanel").GetComponent<UISlideScript>();
    }

    void Start () {
    }

    public void SelectSensor() {
        if (tsos == null) {
            if (!FindSensorToggleObject())
                return;
        }
        List<GameObject> sensorList = new List<GameObject>();
        sensorList.Add(tsos.gameObject);
        list.GetComponent<ListControlScript>().ClearOptions();
        list.GetComponent<ListControlScript>().PopulateList("Sensor", sensorList);
    }

    public bool FindSensorToggleObject() {
        foreach (Transform child in listOpts.transform) {
            ToggleSensorOptionScript ts = child.GetComponent<ToggleSensorOptionScript>();
            if (ts.sensor.sensorID == sensorid) {
                tsos = ts;
                return true;
            }
        }
        return false;
    }

    public void SensorListOn() {
        if (GetComponent<Toggle>().isOn) {
            if (!uS.panelVisible) {
                uS.TogglePanel();
            }
            SelectSensor();
            rend.material = selectedMat;
        }
        else {
            rend.material = mat;
        }
    }
}