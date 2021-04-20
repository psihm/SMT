using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutScript : MonoBehaviour {
    public float timeOut; // Timeout setting in seconds
    private float timeOutTimer = 0.0f;
    private bool timedOut = false;

    public Quaternion rotation;
    public Quaternion cameraRotation;
    public Vector3 spawnPos;
    public Vector3 dashPos;
    GameObject player;
    GameObject mainCamera;
    GameObject smtlogo;
    GameObject minimap;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        smtlogo = GameObject.FindGameObjectWithTag("ToggleMapButton");
        minimap = GameObject.FindGameObjectWithTag("Minimap");
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timeOutTimer += Time.deltaTime;


        // Wake up game if button pressed
        if (Input.anyKeyDown && !Input.GetKeyDown("p"))
        {
            timeOutTimer = 0.0f;
            
            if (timedOut == true)
            {
                // Teleport back to spawn
                player.transform.position = spawnPos;
                player.transform.rotation = rotation;
                mainCamera.transform.rotation = cameraRotation;
                smtlogo.SetActive(true);
                minimap.SetActive(true);
                timedOut = false;
            }
           
        }
        
        // If timer reaches zero, start dashboard screensaver
        if (timeOutTimer > timeOut || Input.GetKeyDown("p"))
        {
            // Teleport to dashboard
            timedOut = true;
            smtlogo.SetActive(false);
            minimap.SetActive(false);
            player.transform.position = dashPos;
            player.transform.rotation = rotation;
            mainCamera.transform.rotation = cameraRotation;
        }
	}

}
