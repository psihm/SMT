using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeSensorScript : SensorInfoScript {
    public float leakThresh = 500000f;
    public float clearThresh = 600000f;
    private float nextActionTime = 0.0f;
    private float waitTime = 0.0f;
    private bool prevTriggered = false;
    //public float periodNew = 20f;

    WaitForSeconds pause;

    void Start()
    {
        //StartCoroutine(LastReadCoroutine());
        if (sensorid != "0")
        {
            waitTime = float.Parse(sensorid) / 200000;
            pause = new WaitForSeconds(waitTime);
        }
           
    }

    void Update () {
        // **look into invokerepeating or fix coroutine repeating to improve performance
        if (Time.time > nextActionTime && sensorid != "0")
        {
            //Debug.Log(sensorid);
            //Debug.Log("Tape update");
            StartCoroutine(LastReadCoroutine());
            nextActionTime = Time.time + refreshTime;
        }
        
    }

    IEnumerator LastReadCoroutine() {
        yield return pause;
        //while (true) {
            if (sas == null) {
                sas = gameObject.AddComponent<SmtApiScript>();
                sas.headers = GameObject.Find("AnalyticsAPI").GetComponent<SmtApiScript>().headers;
                while (sas.headers == null) {
                    yield return sas;
                }
            }

        //Debug.Log("Tape: "+sensorid);

            while (tsos == null) {
                FindSensorToggleObject();
                yield return null;
            }

            yield return StartCoroutine(sas.LoadLastRead(sensorid));
            List<SmtApiResponses.Reading> reList = new List<SmtApiResponses.Reading>(sas.readings);

            if (reList.Count > 0) {
                //Debug.Log(reList[0].raw);
                reading = reList[0];
            }

            // if the previous reading was below leak thresh and current reading is below clear thresh
            if (prevTriggered && (float.Parse(reading.raw) < clearThresh || float.Parse(reading.raw) == 0.00))
            {
                mat = dangerMat;
                rend.material = mat;
            }
            // if reading is below leak thresh
            else if (float.Parse(reading.raw) < leakThresh || float.Parse(reading.raw) == 0.00)
            {
                //Debug.Log("Sensor: " + sensorid + " reading of " + reading.raw + " is less than leak thresh of " + leakValue);
                mat = dangerMat;
                rend.material = mat;
                prevTriggered = true;
            }
            else
            {
                //Debug.Log("Sensor: " + sensorid + " reading of " + reading.raw + " is greater than leak thresh of " + leakValue);
                rend.material = mat;
                prevTriggered = false;
            }
                

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
