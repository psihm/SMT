using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmtApiResponses {
    public class ApiNode : Object {
        
        public string nodeID;
        public string phyID;
        public ApiNode(string newnodeID, string newphyID, string newname) {
            name = newname;
            nodeID = newnodeID;
            phyID = newphyID;
        }
    }
    public class Reading : Object {
        public Reading(string newdataID, string newsensorID, string newraw, string newtimestamp, string newengUnit) {
            dataID = newdataID;
            engUnit = newengUnit;
            raw = newraw;
            sensorID = newsensorID;
            timestamp = newtimestamp;
        }

        public string dataID;
        public string engUnit;
        public string raw;
        public string sensorID;
        public string timestamp;
    }
    public class Sensor : Object {
        public Sensor(string newsensorID, string newsensorName, string newinput, string newsensorTypeID, string newsensorTypeName) {
            sensorID = newsensorID;
            sensorName = newsensorName;
            input = newinput;
            sensorTypeID = newsensorTypeID;
            sensorTypeName = newsensorTypeName;
        }

        public string input;
        public string sensorID;
        public string sensorName;
        public string sensorTypeID;
        public string sensorTypeName;
    }
}

