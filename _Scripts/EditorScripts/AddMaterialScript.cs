using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMaterialScript : MonoBehaviour {
    public Material mat;

	// Use this for initialization
	public void AddMat () {        
        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
            if (rend.sharedMaterial == null) {
                rend.sharedMaterial = mat;
            }                              
        }	
	}
}
