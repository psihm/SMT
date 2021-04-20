using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataText : SensorInfoScript
{
    private float nextActionTime = 0.0f;
    //public float periodNew = 20f;
    private Text readingText;

    void Start()
    {
        readingText = GetComponent<Text>();
        //readingText.text = "testreading";
        //StartCoroutine(LastReadCoroutine());
    }

    void Update()
    {
        // **look into invokerepeating or fix coroutine repeating to improve performance
        if (Time.time > nextActionTime && sensorid != "0")
        {
            StartCoroutine(LastReadCoroutine());
            nextActionTime = Time.time + refreshTime;
        }

    }

    IEnumerator LastReadCoroutine()
    {
        //yield return new WaitForSeconds(10f);
        //while (true) {
        if (sas == null)
        {
            sas = gameObject.AddComponent<SmtApiScript>();
            sas.headers = GameObject.Find("AnalyticsAPI").GetComponent<SmtApiScript>().headers;
            while (sas.headers == null)
            {
                yield return sas;
            }
        }

        //Debug.Log("Tape: "+sensorid);

        while (tsos == null)
        {
            FindSensorToggleObject();
            yield return null;
        }

        yield return StartCoroutine(sas.LoadLastRead(sensorid));
        List<SmtApiResponses.Reading> reList = new List<SmtApiResponses.Reading>(sas.readings);

        if (reList.Count > 0)
        {
            //Debug.Log(reList[0].raw);
            reading = reList[0];
        }

        readingText.text = reading.engUnit.Substring(0,4) + "°C";


        //yield return new WaitForSeconds(refreshTime);
        //}
    }

    /*IEnumerator LastReadCoroutineDelay()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("init");
        //while (true) {
        if (sas == null)
        {
            sas = gameObject.AddComponent<SmtApiScript>();
            sas.headers = GameObject.Find("AnalyticsAPI").GetComponent<SmtApiScript>().headers;
            while (sas.headers == null)
            {
                yield return sas;
            }
        }

        while (tsos == null)
        {
            FindSensorToggleObject();
            yield return null;
        }

        yield return StartCoroutine(sas.LoadLastRead(sensorid));
        List<SmtApiResponses.Reading> reList = new List<SmtApiResponses.Reading>(sas.readings);

        if (reList.Count > 0)
        {
            //Debug.Log(reList[0].raw);
            reading = reList[0];
        }

        if (float.Parse(reading.raw) < leakValue)
        {
            Debug.Log(reading.raw + " is less than " + leakValue);
            rend.material = dangerMat;
        }
        else
        {
            Debug.Log(reading.raw + " is greater than " + leakValue);
            rend.material = mat;
        }


        //yield return new WaitForSeconds(refreshTime);
        //}
    }*/
}
