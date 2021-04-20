using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Apply to empty GameObject and apply ButtonChild to sub-objects
public class ButtonParent : MonoBehaviour
{

    bool showing;
    Texture2D currPic;
    Rect exitButtonRect;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnGUI ()
    {
        if (!showing)
            return;

        GUI.DrawTexture(new Rect(Screen.width/2 - (float)currPic.width/2, Screen.height/6, (float)currPic.width, (float)currPic.height), currPic);

        if (GUI.Button(new Rect(Screen.width/2 - 30,Screen.height/16,60,35), "Close"))
        {
            //Destroy(currPic);
            showing = false;
        }
            
    }

    void ChangePicture (Texture2D newPic)
    {
        showing = true;
        currPic = newPic;
    }


}
