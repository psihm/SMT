using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChild : MonoBehaviour {

    public Texture2D myPic;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown ()
    {
        SendMessageUpwards("ChangePicture", myPic);
    }
}
