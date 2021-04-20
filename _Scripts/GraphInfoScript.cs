using System.Collections.Generic;
using UnityEngine;

// Store and organize sensor lists to graph
public class GraphInfoScript : MonoBehaviour {
    public List<SmtApiResponses.Sensor> sensorstograph;
    public List<string> types;
    public int maxSensors = 10;
    public bool usePercent;

    void Awake() {
        GameObject.Find("AnalyticsAPI").GetComponent<SmtApiScript>().Login();
        sensorstograph = new List<SmtApiResponses.Sensor>();
        types = new List<string>(2);
    }

    public Dictionary<string, List<SmtApiResponses.Sensor>> SensorTypeLists() {
        Dictionary<string, List<SmtApiResponses.Sensor>> typedic = new Dictionary<string, List<SmtApiResponses.Sensor>>();
        foreach (string ty in types) {
            typedic.Add(ty, new List<SmtApiResponses.Sensor>());
        }

        // Compare the first word of each sensor's type with currently selected sensors
        foreach (SmtApiResponses.Sensor s in sensorstograph) {
            usePercent = false;
            string sen = s.sensorTypeName;
            //Debug.Log(sen);
            if (sen.Contains("Moisture (%)"))
                usePercent = true;
            string firstWord = sen.IndexOf(" ") > -1
              ? sen.Substring(0, sen.IndexOf(" "))
              : sen;

            // Check type unknown or equation, set these to Compression
            // May want to fix later if types become renamed
            if (firstWord.ToLower().Contains("unknown") || firstWord.ToLower().Contains("equation")) {
                firstWord = "Compression";
            }

            if (types.Contains(firstWord)) {
                string addType = types.Find(str => str.Contains(firstWord));
                typedic[addType].Add(s);
            }
        }
        return typedic;
    }
}
