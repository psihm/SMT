using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamburgerScript : MonoBehaviour {
    //animator reference
    private Animator anim;
    public bool panelVisible;

    // Use this for initialization
    void Start() {
        //get the animator component
        //anim = gameObject.GetComponent<Animator>();
        ////disable it on start to stop it from playing the default animation
        //anim.enabled = false;
        panelVisible = false;
    }
    
    public void SlideIn() {
        //enable the animator component
        anim.enabled = true;
        //play the Slidein animation
        anim.Play("HamburgerSlideIn");
        //set the isPaused flag to true to indicate that the game is paused
        panelVisible = true;

    }

    public void SlideOut() {
        anim.enabled = true;
        //set the isPaused flag to false to indicate that the game is not paused
        panelVisible = false;
        //play the SlideOut animation
        anim.Play("HamburgerSlideOut");
    }

    public void TogglePanel() {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        //if (panelVisible) {
        //    SlideOut();
        //}
        //else {
        //    SlideIn();
        //}
    }
}
