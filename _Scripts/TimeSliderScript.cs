using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSliderScript : MonoBehaviour {
    public GameObject graph;
    public GraphScript gs;
    public Slider slider;
    public bool canZoom = false;
	// Use this for initialization
	void Start () {
        gs = graph.GetComponent<GraphScript>();
        slider = GetComponent<Slider>();
    }

    void LateUpdate() {
        if (canZoom) {
            float deltaMagnitudeDiff = 0f;
            if (Input.touchCount >= 2) {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0.0f || deltaMagnitudeDiff > 0f) {
                slider.value += 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f || deltaMagnitudeDiff < 0f) {
                slider.value -= 1;
            }
        }   
    }

    public void CanZoom() {
        canZoom = true;
    }

    public void CannotZoom() {
        canZoom = false;
    }

    public void PanTimeScale() {
        gs.SlideTimeAxis((double)slider.value);
    }

    public void ZoomTimeScale() {
        gs.ZoomTimeAxis((double)slider.value);
    }
}
