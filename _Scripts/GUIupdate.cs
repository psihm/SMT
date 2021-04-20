using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIupdate : GUIscript {
    GUIStyle guiStyle = new GUIStyle();
    bool guiOn = true;
    private GameObject[] dashboardArray;

    void Awake()
    {
        dashboardArray = GameObject.FindGameObjectsWithTag("Dashboard");

    }


    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Player")
        {
            guiOn = false;
            foreach (GameObject go in dashboardArray)
                go.SetActive(true);
        }
            
    }

    private void OnTriggerExit(Collider obj)
    {
        if (obj.tag == "Player")
        {
            guiOn = true;
            foreach (GameObject go in dashboardArray)
                go.SetActive(false);
        }
            
    }

    // GUI labels overlay (called multiple times per frame)
    void OnGUI()
    {
        if (guiOn)
        {
            GUI.contentColor = Color.black;
            guiStyle.fontSize = Screen.height / 30;
            GUI.Label(new Rect((Screen.width / 20) * 2, 0, 200, 50), "Weather: " + weather, guiStyle);
            GUI.Label(new Rect((Screen.width / 20) * 7, 0, 200, 50), "Temperature: " + temperature + "°C", guiStyle);
            GUI.Label(new Rect((Screen.width / 20) * 11, 0, 200, 50), "Humidity: " + relativeHumidity, guiStyle);
            GUI.Label(new Rect((Screen.width / 20) * 14, 0, 200, 50), "Wind: " + windDirection + " at " + windKPH + " km/h", guiStyle);

        }
    }

}
