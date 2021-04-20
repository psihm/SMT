using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WWtemp : GUIscript
{
    private Text readingText;



	// Use this for initialization
	void Start ()
    {
        readingText = GetComponent<Text>();
        InvokeRepeating("UpdateWeather", 5.0f, 150.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void UpdateWeather()
    {
        readingText.text = temperature + "°C";
    }



}
