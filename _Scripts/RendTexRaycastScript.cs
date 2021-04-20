    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RawImage))]
public class RendTexRaycastScript : MonoBehaviour {
    public Camera rendTexCam; //Assign Render Texture's Camera in Inspector.

    private int sensorLayerMask = 1 << 12;
    private UISlideScript uS;

    // Use this for initialization
    void Awake() {
        uS = GameObject.Find("UIPanel").GetComponent<UISlideScript>();
    }

    // Translate clicked position on screen, to position on render texture camera viewport
    // then raycast from rendtex camera to check for sensor hits
    public void CheckHit() {
        Texture tex = GetComponent<RawImage>().texture;
        RectTransform recttrans = transform.GetComponent<RectTransform>();
        // Find local coordinates, then turn into ratio of width and height.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(recttrans, Input.mousePosition, null, out localPoint);
        // Shift so 0,0 is bottom left, needed for viewport raycast coordinate.
        float px = (localPoint.x / recttrans.rect.width) + 0.5f;
        float py = (localPoint.y / recttrans.rect.height) + 0.5f;

        RaycastHit hitInfo = new RaycastHit();
        // Cast out from renTexCam viewport at relative position on screen
        Ray ray = rendTexCam.ViewportPointToRay(new Vector3(px, py, 0));
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, sensorLayerMask);

        // If sensor hit, toggle the sensor and bring up sensor panel.
        if (hit) {
            //Debug.Log("Hit!");
            if (!uS.panelVisible) {
                uS.TogglePanel();
            }
            hitInfo.transform.GetComponentInChildren<Toggle>().isOn = true;
        }
    }
}
