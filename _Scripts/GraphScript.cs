using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Responsible for graphing methods, using GraphMaker assets
public class GraphScript : MonoBehaviour {
    public GameObject graphPrefab; // Assign in inspector, EmptyLineGraphPrefab
    public GameObject graphobj; // Object graph component is attached
    public WMG_Axis_Graph graph;
    public List<WMG_Series> seriesList;
    public GameObject api;
    public bool isRunning;

    public static float lastReading;

    public DateTime dateCenter; // The date to center the graph on
    public DateTime currentDateCenter;
    public double daysBeforeAfter; // Number of days on the graph, before and after the center date

    private SmtApiScript apiscript;
    private GraphInfoScript gis;
    private List<List<SmtApiResponses.Reading>> allReadings; // Sensor readings
    private Vector3 _mouseOrigin;

    string stime = "00:00:00";
    string etime = "23:59:59";

    private List<Color> colors; // Line colors
    
	// Use this for initialization
	void Awake () {
        isRunning = false;
      
        apiscript = GameObject.Find("AnalyticsAPI").GetComponent<SmtApiScript>();
        gis = transform.parent.GetComponent<GraphInfoScript>();
        Color orange = new Color32(255, 140, 0, 255);
        Color pink = new Color32(255, 105, 180, 255);

        colors = new List<Color>();

        colors.Add(Color.blue);
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.yellow);
        colors.Add(Color.cyan);
        colors.Add(Color.magenta);
        colors.Add(orange);
        colors.Add(pink);
        colors.Add(Color.white);
        colors.Add(Color.grey);

        allReadings = new List<List<SmtApiResponses.Reading>>();
    }

    public void CreateGraph() {    
        // If no sensors to graph, return.
        if (gis.sensorstograph.Count <= 0 || gis.types.Count <= 0)
            return;

        // Instantiate the graph prefab, based on a copy of the included Line Graph prefab
        graphobj = Instantiate(graphPrefab, transform);
        graph = graphobj.GetComponent<WMG_Axis_Graph>();

        // Need a series to set up axis
        graph.addSeries();

        // Custom labeler
        graph.theTooltip.tooltipLabeler = dateTooltipLabeler;

        // Default the graph to the past month
        daysBeforeAfter = 15;
        dateCenter = DateTime.Now.AddDays(-daysBeforeAfter);

        SetupXAxis(dateCenter.AddDays(-daysBeforeAfter), DateTime.Now);
        SetupYAxis();

        // Remove any temporary or previous line series
        while (graph.lineSeries.Count > 0) {
            graph.deleteSeries();
        }
        StartCoroutine(GraphValues());
    }

    // Place graph value points on the graph. Pulls data for each sensor.
    // Y values are engunit, x values are translated to hours based on timestamp.
    IEnumerator GraphValues() {
        isRunning = true;
        Dictionary<string, List<SmtApiResponses.Sensor>> typedic = gis.SensorTypeLists();
        DateTime datenow = DateTime.Now;
        DateTime startdate = dateCenter.AddDays(-daysBeforeAfter);
        startdate = new DateTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
        string edate = datenow.Year.ToString() + "-" + datenow.Month.ToString() + "-" + datenow.Day.ToString();
        edate.Replace("/", "-");
        string sdate = startdate.Year.ToString() + "-" + startdate.Month.ToString() + "-" + startdate.Day.ToString();
        sdate.Replace("/", "-");

        int i = 0;
        // Check which and how many types the graph needs
        List<string> keys = new List<string>(typedic.Keys);
        for (int k=0; k < keys.Count; k++) {
            string key = keys[k];
            List<SmtApiResponses.Sensor> graphSensors = typedic[key];
            // Assign axis based on type
            foreach (SmtApiResponses.Sensor s in graphSensors) {
                WMG_Series series = graph.addSeries();
                if (k > 0) {
                    series.useSecondYaxis = true;
                }
                else {
                    series.useSecondYaxis = false;
                }
                series.seriesName = "Loading Sensor...";
                // Get sensor readings
                yield return StartCoroutine(apiscript.LoadReading(s.sensorID, sdate, edate, stime, etime));
                List<SmtApiResponses.Reading> readings = new List<SmtApiResponses.Reading>(apiscript.readings);
                List<Vector2> vals = new List<Vector2>();

                series.seriesName = s.sensorName;
                Color c = colors[i % colors.Count];
                series.lineColor = c;
                series.pointColor = c;
                i++;

                // Add plot points for each reading in the sensor
                foreach (SmtApiResponses.Reading r in readings) {
                    // Parse reading y value.
                    float y = float.Parse(r.engUnit);
                    //lastReading = float.Parse(r.engUnit);
                    //Debug.Log(r.engUnit);
                    // Parse reading x value from timestamp
                    float x = ParseTime(r) + ParseDate(r, startdate);
                    if (x >= 0 && x <= graph.xAxis.AxisMaxValue) {
                        vals.Add(new Vector2(x, y));
                    }                    
                }
                if (vals.Count <= 0) {
                    series.seriesName += ": No Data for Date";
                }

                series.pointValues.SetList(vals);

                allReadings.Add(new List<SmtApiResponses.Reading>(readings));

                //Debug.Log(readings[readings.Count - 1]);

                seriesList.Add(series);
                yield return s;
            }
        }
        ChangeTimeAxis(dateCenter, daysBeforeAfter);
        isRunning = false;
    }

    private void ChangeTimeAxis(DateTime dateChange, double dayChange) {
        daysBeforeAfter = dayChange;
        currentDateCenter = dateChange;
        DateTime startdate = dateChange.Subtract(TimeSpan.FromDays(daysBeforeAfter));
        SetupXAxis(startdate, dateChange.AddDays(daysBeforeAfter));

        PlaceValues();
    }

    // Place x and y values for readings onto graph.
    private void PlaceValues() {
        for (int k = 0; k < seriesList.Count; k++) {
            List<Vector2> vals = new List<Vector2>();
            foreach (SmtApiResponses.Reading r in allReadings[k]) {
                float y = float.Parse(r.engUnit);
                float x = ParseTime(r) + ParseDate(r, currentDateCenter.AddDays(-daysBeforeAfter));
                if (x >= 0 && x <= graph.xAxis.AxisMaxValue) {
                    vals.Add(new Vector2(x, y));
                }
            }
            seriesList[k].pointValues.SetList(vals);
        }
    }

    // Change graph centered date and replace plot points.
    public void SlideTimeAxis(double dayChange) {
        currentDateCenter = dateCenter.AddDays(dayChange);
        SetupXAxis(currentDateCenter.AddDays(-daysBeforeAfter), currentDateCenter.AddDays(daysBeforeAfter));
        PlaceValues();
    }

    // Change amount of days before and after center date, replace plot points
    public void ZoomTimeAxis(double dayBAChange) {
        if (dayBAChange > 15 || dayBAChange < 0)
            return;
        daysBeforeAfter = dayBAChange;
        SetupXAxis(currentDateCenter.AddDays(-daysBeforeAfter), currentDateCenter.AddDays(daysBeforeAfter));
        PlaceValues();
    }

    // Set up graph prefab x axis
    void SetupXAxis(DateTime start, DateTime end) {
        TimeSpan ts = end.Subtract(start);
        int daysago = ts.Days;

        graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
        // X axis in days
        if (daysago > 1) {
            graph.xAxis.AxisTitleString = "Day";
            graph.xAxis.SetLabelsUsingMaxMin = false;
            graph.xAxis.AxisNumTicks = (int)daysago + 2;
            graph.xAxis.AxisMaxValue = (float)((daysago + 1) * 24);
            graph.xAxis.AxisMinValue = 0;
            // Label x axis with date text
            for (int j = 0; j < graph.xAxis.axisLabels.Count; j++) {
                DateTime dt = start.AddDays(j);
                if (daysago > 14) {
                    if (j % 2 == 0)
                        graph.xAxis.axisLabels[j] = dt.ToString("dd") + "/" + dt.ToString("MM");
                    else
                        graph.xAxis.axisLabels[j] = "";
                }
                else {
                    graph.xAxis.axisLabels[j] = dt.ToString("dd") + "/" + dt.ToString("MM");
                }
            }
        }
        // X axis in hours
        else {
            graph.xAxis.AxisTitleString = "Hour\n" + currentDateCenter.Date.ToString("d");
            graph.xAxis.SetLabelsUsingMaxMin = true;
            graph.xAxis.AxisNumTicks = 26;
            graph.xAxis.AxisMaxValue = 25;
            graph.xAxis.AxisMinValue = 0;
        }
        graph.xAxis.UpdateAxesLabels();
        graph.xAxis.UpdateAxesGridsAndTicks();
    }

    // Setup Y axis of graph
    void SetupYAxis() {
        string type1 = gis.types[0];
        string unit1 = "";
        // Switch title based on sensor types
        switch (type1) {
            case "Moisture":
                if (gis.usePercent)
                    unit1 = "%";
                else
                    unit1 = "kΩ";
                break;
            case "Humidity":
                unit1 = "%";
                break;
            case "Equation":
            case "Unknown":     
            case "Compression":
                type1 = "Heat Flux";
                unit1 = "W/m²";
                break;
            case "Temperature":
                unit1 = "°C";
                break;         
            default:
                break;
        }
        graph.yAxis.AxisTitleString = type1 + " " + unit1;
        graph.yAxis.SetLabelsUsingMaxMin = true;
        graph.yAxis.LabelType = WMG_Axis.labelTypes.ticks;
        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.yAxis.MinAutoGrow = true;
        graph.yAxis.MinAutoShrink = true;

        if (gis.types.Count > 1) {
            string type2 = gis.types[1];
            string unit2 = "";
            switch (type2) {
                case "Moisture":
                    unit1 = "kΩ";
                    break;
                case "Humidity":
                    unit2 = "%";
                    break;
                case "Equation":
                case "Compression":
                    type2 = "Compression";
                    unit2 = "mm";
                    break;
                case "Temperature":
                    unit2 = "°C";
                    break;
                default:
                    break;
            }

            graph.axesType = WMG_Axis_Graph.axesTypes.DUAL_Y;
            graph.yAxis2.AxisTitleString = type2 + " " + unit2;
            graph.yAxis2.SetLabelsUsingMaxMin = true;
            graph.yAxis2.LabelType = WMG_Axis.labelTypes.ticks;
            graph.yAxis2.MaxAutoGrow = true;
            graph.yAxis2.MaxAutoShrink = true;
            graph.yAxis2.MinAutoGrow = true;
            graph.yAxis2.MinAutoShrink = true;
            graph.yAxis2.numDecimalsAxisLabels = graph.yAxis.numDecimalsAxisLabels;
            graph.yAxis2.AxisNumTicks = graph.yAxis.AxisNumTicks;
            graph.yAxis2.AxisTitleOffset = graph.yAxis.AxisTitleOffset;
            graph.yAxis2.AxisTitleFontSize = graph.yAxis.AxisTitleFontSize;
        }
    }

    public void DestroyGraph() {
        Destroy(graphobj);
    }
    
    float ParseTime(SmtApiResponses.Reading reading) {
        string time = reading.timestamp.Substring(11, 8);
        return float.Parse(ParseHour(time)) + ((float.Parse(ParseMinute(time))) / 60);
    }

    // Returns the hours difference from: reading.timestamp - sdate
    float ParseDate(SmtApiResponses.Reading reading, DateTime sdate) {
        DateTime readingDT = new DateTime(int.Parse(ParseYear(reading.timestamp)), int.Parse(ParseMonth(reading.timestamp)), int.Parse(ParseDay(reading.timestamp)));
        TimeSpan retTime = readingDT.Subtract(sdate);
        return (float)retTime.TotalHours;
    }

    string ParseYear(string date) {
        if (date.Length < 10)
            return null;
        return date.Substring(0, 4);
    }

    string ParseMonth(string date) {
        if (date.Length < 10)
            return null;
        return date.Substring(5, 2);
    }

    string ParseDay(string date) {
        if (date.Length < 10)
            return null;
        return date.Substring(8, 2);
    }

    string ParseHour(string time) {
        return time.Substring(0, 2);
    }

    public string ParseMinute(string time) {
        return time.Substring(3, 2);
    }

    // Custom tooltip labels, show time of day
    string dateTooltipLabeler(WMG_Series aSeries, WMG_Node aNode) {
        // Find out the point value data for this node
        Vector2 nodeData = aSeries.getNodeValue(aNode);
        float numberToMult = Mathf.Pow(10f, aSeries.theGraph.tooltipNumberDecimals);
        float hours = (Mathf.Round(nodeData.x * numberToMult) / numberToMult);
        int min = (int)((hours - (int)hours) * 60);
        string minutes = min.ToString();
        if (min < 10) {
            minutes = "0" + minutes;
        }
        string nodeX = ( ((int)hours % 24).ToString() + ":" + minutes).ToString();
        string nodeY = (Mathf.Round(nodeData.y * numberToMult) / numberToMult).ToString();

        // Determine the tooltip text to display
        string textToSet;
        if (aSeries.seriesIsLine) {
            textToSet = "(" + nodeX + ", " + nodeY + ")";
        }
        else {
            textToSet = nodeY;
        }
        if (aSeries.theGraph.tooltipDisplaySeriesName) {
            textToSet = aSeries.seriesName + ": " + textToSet;
        }
        return textToSet;
    }
}
