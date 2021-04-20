﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSunny : GUIscript
{
    Renderer rend;

	// Use this for initialization
	void Start ()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        InvokeRepeating("UpdateIcon", 5.0f, 150.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void UpdateIcon ()
    {
        if (clear)
            rend.enabled = true;
        else
            rend.enabled = false;
    }
}
