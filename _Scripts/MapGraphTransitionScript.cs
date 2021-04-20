using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGraphTransitionScript : MonoBehaviour {

    public GameObject graph;
    public GameObject map;
    public Slider zoomSlider; // Assign in Inspector
    public Slider panSlider; // Assign in Inspector

    public void GraphOn() {
        zoomSlider.gameObject.SetActive(false);
        panSlider.gameObject.SetActive(false);
        graph.SetActive(true);
        graph.GetComponent<GraphScript>().CreateGraph();
        StartCoroutine(SliderOn());
        map.SetActive(false);
    }

    public void GraphOff() {
        graph.GetComponent<GraphScript>().DestroyGraph();
        graph.SetActive(false);
        map.SetActive(true);
        zoomSlider.gameObject.SetActive(false);
        panSlider.gameObject.SetActive(false);
    }

    IEnumerator SliderOn() {
        while (graph.GetComponent<GraphScript>().isRunning)
            yield return null;
        zoomSlider.value = zoomSlider.maxValue;
        panSlider.value = 0;
        zoomSlider.gameObject.SetActive(true);
        panSlider.gameObject.SetActive(true);
    }
}
