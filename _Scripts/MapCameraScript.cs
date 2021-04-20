using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraScript : MonoBehaviour {
    GameObject player;

    public float camHeight = 4.2f; // Height above player for the roof
    public float secondFloorHeight = 20f; // Masking roof elements so any really high value works
    public float firstFloorHeight = 5f;
    public float roofHeight;
    public float secondFloorThresh = 4f; // y value to move camera to second floor height
    public float roofThresh = 9f; // y value to move camera to roof height

    int withRoof;
    int withoutRoof;
	// Use this for initialization
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        withoutRoof = GetComponent<Camera>().cullingMask;
        withRoof = GetComponent<Camera>().cullingMask | (1 << LayerMask.NameToLayer("Roof"));        
    }
	void Update () {
        float height = firstFloorHeight;
        if (player.transform.position.y > roofThresh) {
            GetComponent<Camera>().cullingMask = withRoof;
            // Follow player height is nice for when player is inside roof room then walks outside.
            transform.position = new Vector3(transform.position.x, player.transform.position.y + camHeight, transform.position.z);
        }
        else {
            GetComponent<Camera>().cullingMask = withoutRoof;
            if (player.transform.position.y > secondFloorThresh) {
                height = secondFloorHeight;
            }
            else {
                height = firstFloorHeight;
            }
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
    }
}
