using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpScript : MonoBehaviour {
    public Vector3 warpVector;
    GameObject player;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Warp() {
        if (warpVector == Vector3.zero)
            Debug.Log("Warp Vector is (0,0,0)!");
        player.transform.position = warpVector;
    }
}
