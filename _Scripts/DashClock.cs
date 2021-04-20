using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DashClock : GUIscript
{
    private Text readingText;
    DateTime time;
    string hour;
    string minute;
    string second;

    // Use this for initialization
    void Start()
    {
        readingText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time = DateTime.Now;
        hour = time.Hour.ToString().PadLeft(2, '0');
        minute = time.Minute.ToString().PadLeft(2, '0');
        second = time.Second.ToString().PadLeft(2, '0');

        readingText.text = hour + ":" + minute + ":" + second;
    }



}
