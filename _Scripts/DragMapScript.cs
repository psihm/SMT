using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMapScript : MonoBehaviour {
    public bool canPan = false;
    public bool canZoom = false;
    //The default distance of the camera from the target.
    public float _distance;
    public float maxDistance = 100.0f;
    public float minDistance = 5.0f;
    public float maxX = 0;
    public float minX = 0;
    public float maxZ = 0;
    public float minZ = 0;

    //Control the speed of zooming and dezooming.
    public float _zoomStep = 1.0f;

    //The speed of the camera. Control how fast the camera will rotate.
    public float _xSpeed = 3f;
    public float _ySpeed = 3f;

    public float _panSpeed = 0.01f;

    //The position of the cursor on the screen. Used to rotate the camera.
    //private float _x = 0.0f;
    //private float _y = 0.0f;

    //Distance vector. 
    private Vector3 _distanceVector;
    private Vector3 _mouseOrigin;

    private Camera cam;

    // Use this for initialization
    void Awake () {
        cam = GetComponent<Camera>();
        if (cam.orthographic)
            _distance = cam.orthographicSize;
        else
            _distance = cam.transform.position.y;
	}

    // Update is called once per frame
    void LateUpdate() {
        if (canPan) {
            Pan();
        }
        if (canZoom) {
            Zoom();
        }
    }

    public void CanPan() {
        canPan = true;
    }

    public void CannotPan() {
        canPan = false;
    }

    public void CanZoom() {
        canZoom = true;
    }

    public void CannotZoom() {
        canZoom = false;
    }

    /**
     * Zoom or dezoom depending on the input of the mouse wheel.
     */
    void Zoom() {
        float deltaMagnitudeDiff = 0f;
        // If there are two touches on the device...
        if (Input.touchCount == 2) {
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
            this.ZoomOut();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f || deltaMagnitudeDiff < 0f) {
            this.ZoomIn();
        }

    }

    /**
     * Reduce the distance from the camera to the target and
     * update the position of the camera (with the Rotate function).
     */
    void ZoomIn() {
        float newDist = _distance - _zoomStep;
        if (newDist >= minDistance) {
            _distance -= _zoomStep;
        }

        if (cam.orthographic)
            cam.orthographicSize = _distance;
        else
            cam.transform.position = new Vector3(cam.transform.position.x, _distance, cam.transform.position.z);
    }

    /**
     * Increase the distance from the camera to the target and
     * update the position of the camera (with the Rotate function).
     */
    void ZoomOut() {
        float newDist = _distance + _zoomStep;
        if (newDist <= maxDistance) {
            _distance += _zoomStep;
        }

        if (cam.orthographic)
            cam.orthographicSize = _distance;
        else
            cam.transform.position = new Vector3(cam.transform.position.x, _distance, cam.transform.position.z);
    }

    void Pan() {
        if (Input.GetMouseButtonDown(0)) {
            _mouseOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            float xpos = cam.transform.position.x + (_mouseOrigin.x - Input.mousePosition.x) * _panSpeed;
            if (xpos > maxX || xpos < minX) {
                xpos = cam.transform.position.x;
            }

            float zpos = cam.transform.position.z + (_mouseOrigin.y - Input.mousePosition.y) * _panSpeed;
            if (zpos > maxZ || zpos < minZ) {
                zpos = cam.transform.position.z;
            }
            cam.transform.position = new Vector3(xpos, cam.transform.position.y, zpos);
            _mouseOrigin = Input.mousePosition;
        }
    }
}
