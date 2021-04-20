// By: Hamza Mustapha
// Date Decemeber 6, 2020

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

// Access SMT Analytics
public class SmtApiScript : MonoBehaviour {

    string LOGIN_URL = "https://analytics.smtresearch.ca/api/?action=login&user_username=hamza&user_password=smt";
    string REQUEST_NODE_LIST_URL = "https://analytics.smtresearch.ca/api/?action=listNode&jobID={0}";
    string REQUEST_SENSOR_LIST_URL = "https://analytics.smtresearch.ca/api/?action=listSensor&nodeID={0}";
    private string LAST_READING_URL = "https://analytics.smtresearch.ca/api/?action=lastReading&sensorID={0}&date={1}&time={2}";
    string REQUEST_SENSOR_DATA_URL_FORMAT = "https://analytics.smtresearch.ca/api/?action=listSensorData&sensorID={0}&startDate={1}&endDate={2}&startTime={3}&endTime={4}";
    string LOGOUT_URL = "https://analytics.smtresearch.ca/api/?action=logout";
    public string JOBID = "3049"; // RRC Innovation Centre 
    public Dictionary<string, string> headers;

    public bool isRunning;

    public List<SmtApiResponses.ApiNode> nodes;
    public List<SmtApiResponses.Sensor> sensors;
    public List<SmtApiResponses.Reading> readings;

    public GameObject errorBox;
    public float timeOut = 7f;

    // Use this for initialization
    void Awake() {
        nodes = new List<SmtApiResponses.ApiNode>();
        sensors = new List<SmtApiResponses.Sensor>();
        readings = new List<SmtApiResponses.Reading>();
        isRunning = false;
    }

    List<SmtApiResponses.Sensor> ParseSensorsXML(string xml) {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode node = doc.DocumentElement.SelectSingleNode("/result/sensors");

        if (node == null) {
            return null;
        }

        List<SmtApiResponses.Sensor> retList = new List<SmtApiResponses.Sensor>();
        foreach (XmlNode sennode in node.ChildNodes) {
            retList.Add(new SmtApiResponses.Sensor(sennode["sensorID"].InnerText, sennode["name"].InnerText, sennode["input"].InnerText, sennode["sensorTypeID"].InnerText, sennode["sensorTypeName"].InnerText));
        }
        return retList;
    }

    List<SmtApiResponses.ApiNode> ParseApiNodesXML(string xml) {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode node = doc.DocumentElement.SelectSingleNode("/result/nodes");
        if (node == null) {
            return null;
        }

        List<SmtApiResponses.ApiNode> retList = new List<SmtApiResponses.ApiNode>();

        foreach (XmlNode sennode in node.ChildNodes) {
            retList.Add(new SmtApiResponses.ApiNode(sennode["nodeID"].InnerText, sennode["phyID"].InnerText, sennode["name"].InnerText));
        }
        return retList;
    }

    List<SmtApiResponses.Reading> ParseReadingsXML(string xml) {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode node = doc.DocumentElement.SelectSingleNode("/result/readings");
        if (node == null) {
            return null;
        }

        List<SmtApiResponses.Reading> retList = new List<SmtApiResponses.Reading>();

        foreach (XmlNode sennode in node.ChildNodes) {
            retList.Add(new SmtApiResponses.Reading(sennode["dataID"].InnerText, sennode["sensorID"].InnerText, sennode["raw"].InnerText, sennode["timestamp"].InnerText, sennode["engUnit"].InnerText));
        }
        return retList;
    }

    public void Login() {
        while (isRunning) {
            //yield return null;
        }

        isRunning = true;
        UnityWebRequest www = new UnityWebRequest(LOGIN_URL);
        while (!www.isDone) {
            //yield return www;
        }

        if (www.error == null) {
            Debug.Log("WWW Ok!: " + www.isDone);
        }
        else {
            Debug.Log("WWW Error: " + www.error);
            StartCoroutine(ReportError(www.error));
            return;
            //yield break;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(LOGIN_URL);
        XmlNodeList sessid = doc.GetElementsByTagName("PHPSESSID");
        string sessionID = sessid[0].InnerText;
        //Debug.Log(sessionID);

        WWWForm wwwform = new WWWForm();

        headers = wwwform.headers;
        headers.Add("Cookie", "PHPSESSID=" + sessionID);
        isRunning = false;
    }

    public void LogoutWrapper() {
        if (!isRunning) {
            StartCoroutine(Logout());
        }
    }

    IEnumerator Logout() {
        while (isRunning) {
            yield return null;
        }

        isRunning = true;
        UnityWebRequest www = new UnityWebRequest(LOGOUT_URL);
        while (!www.isDone) {
            yield return www;
        };

        if (www.error == null) {
            Debug.Log("WWW Ok!: " + www.isDone);
        }
        else {
            Debug.Log("WWW Error: " + www.error);
            yield return StartCoroutine(ReportError(www.error));
            yield break;
        }
        isRunning = false;
    }

    public void LoadNodeListWrapper() {
        if (!isRunning) {
            StartCoroutine(LoadNodeList());
        }
    }

    IEnumerator LoadNodeList() {
        while (isRunning) {
            yield return null;
        }
        isRunning = true;
        string url = string.Format(REQUEST_NODE_LIST_URL, JOBID);
        UnityWebRequest www = new UnityWebRequest(url);
        while (!www.isDone) {
            yield return www;
        };

        if (www.error == null) {
            Debug.Log("WWW Ok!: " + www.isDone);
            if (url.Contains("login required")) {
                isRunning = false;
                Login();
                yield return www;//StartCoroutine(Login());

                isRunning = true;
                Debug.Log("Retry GetNodeList");

                www = new UnityWebRequest(url);
                while (!www.isDone) {
                    yield return www;
                };

                if (www.error == null) {
                    //Debug.Log("WWW Ok!: " + www.text);
                    if (url.Contains("login required")) {
                        yield return StartCoroutine(ReportError("Server error! Please try again later."));
                        isRunning = false;
                        yield break;
                    }
                }
                else {
                    Debug.Log("WWW Error: " + www.error);
                    yield return StartCoroutine(ReportError(www.error));
                    isRunning = false;
                    yield break;
                }
            }
        }

        //java.util.ArrayList rawnodes = parser.parse(www.text, ApiResponses.responseTypes.NODES);

        nodes.Clear();
        foreach (SmtApiResponses.ApiNode node in ParseApiNodesXML(url)) {
            nodes.Add(node);
        }
        isRunning = false;
    }

    public void LoadSensorList(string nodeid) {
        string url = string.Format(REQUEST_SENSOR_LIST_URL, nodeid);
        //Debug.Log("GetSensorList");

        UnityWebRequest www = new UnityWebRequest(url);
        while (!www.isDone) ;

        if (www.error == null) {
        }
        else {
            Debug.Log("WWW Error: " + www.error);
            //StartCoroutine(ReportError(www.error));
            return;
        }
        sensors.Clear();
        List<SmtApiResponses.Sensor> rawsensors = ParseSensorsXML(url);
        if (rawsensors == null) {
            Debug.Log("WWW Error: " + www.isDone);
            //StartCoroutine(ReportError(www.text));
            return;
        }

        sensors.Clear();
        foreach (SmtApiResponses.Sensor sensor in rawsensors) {
            sensors.Add(sensor);
            //Debug.Log("Sensor: " + sensor.sensorName + " ID: " + sensor.sensorID + " TypeName: " + sensor.sensorTypeName + " TypeID: " + sensor.sensorTypeID);
        }
    }

    public void LoadReadingWrapper(string sid, string sdate, string edate, string stime, string etime) {
        if (!isRunning)
            StartCoroutine(LoadReading(sid, sdate, edate, stime, etime));
    }

    public IEnumerator LoadReading(string sid, string sdate, string edate, string stime, string etime) {
        //while (isRunning) {
        //    yield return null;
        //}

        //isRunning = true;
        //Debug.Log("Start Load: " + sid);
        string[] sensortoread = new string[] { sid, sdate, edate, stime, etime };
        string url = string.Format(REQUEST_SENSOR_DATA_URL_FORMAT, sensortoread);

        UnityWebRequest www = new UnityWebRequest(url);
        while (!www.isDone) {
            yield return www;
        };
        if (www.error == null) {
        }
        else {
            Debug.Log("WWW Error: " + www.error);
            //isRunning = false;
            yield return StartCoroutine(ReportError(www.error));
            //isRunning = false;
            yield break;
        }

        //ApiResponseParser parser = new ApiResponseParser();
        //java.util.ArrayList rawreading = parser.parse(www.text, ApiResponses.responseTypes.SENSORDATA);

        readings.Clear();
        foreach (SmtApiResponses.Reading reading in ParseReadingsXML(url)) {
            readings.Add(reading);
            //yield return readings;
        }
        isRunning = false;
    }

    public void LoadLastReadWrapper(string sensorID) {
        if (!isRunning)
            StartCoroutine(LoadLastRead(sensorID));
    }

    public IEnumerator LoadLastRead(string sensorID) {
        while (isRunning) {
            yield return null;
        }

        // Get the current date.
        DateTime datenow = DateTime.Now;

        string date = datenow.Year.ToString() + "-" + datenow.Month.ToString() + "-" + datenow.Day.ToString();

        date.Replace("/", "-");
        string time = datenow.TimeOfDay.ToString();
        isRunning = true;
        string[] sensortoread = new string[] { sensorID, date, time };
        string url = string.Format(LAST_READING_URL, sensortoread);

        UnityWebRequest www = new UnityWebRequest(url);
        while (!www.isDone) {
            yield return www;
        };

        if (string.IsNullOrEmpty(www.error)) {
            //Debug.Log("WWW Ok!: " + www.text);
            if (url.Contains("login required")) {
                isRunning = false;
                Login();
                yield return www;// StartCoroutine(Login());

                isRunning = true;
                Debug.Log("Retry LastRead");

                www = new UnityWebRequest(url);
                while (!www.isDone) {
                    yield return www;
                };

                if (www.error == null) {
                    //Debug.Log("WWW Ok!: " + www.text);
                    if (url.Contains("login required")) {
                        yield return StartCoroutine(ReportError("Server error! Please try again later."));
                        isRunning = false;
                        yield break;
                    }
                }
                else {
                    Debug.Log("WWW Error: " + www.error);
                    isRunning = false;
                    yield return StartCoroutine(ReportError(www.error));
                    isRunning = false;
                    yield break;
                }
            }
        }

        else {
            Debug.Log("WWW Error: " + www.error);
            isRunning = false;
            yield return StartCoroutine(ReportError(www.error));
            isRunning = false;
            yield break;
        }

        //ApiResponseParser parser = new ApiResponseParser();
        //java.util.ArrayList rawreading = parser.parse(www.text, ApiResponses.responseTypes.SENSORDATA);

        readings.Clear();
        foreach (SmtApiResponses.Reading reading in ParseReadingsXML(url)) {
            readings.Add(reading);
        }
        isRunning = false;
        yield return www;
    }

    public IEnumerator ReportError(string errormsg) {
        Debug.Log("Error reported: " + errormsg);
        if (errorBox != null) {
            errorBox.SetActive(true);
            //errorText.text = "Error!: " + errormsg;
            errorBox.GetComponentInChildren<Text>().text = "Error!: " + errormsg + "\nPlease try restarting application later";
            //yield return new WaitForSeconds(5F);
            //errorBox.SetActive(false);
            //SceneManager.LoadScene("Load");
        }
    yield return null;
    }
}
