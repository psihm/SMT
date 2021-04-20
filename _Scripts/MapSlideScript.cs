using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Slides map panel UI
public class MapSlideScript : MonoBehaviour {
    //animator reference
    private Animator anim;
    public bool panelVisible;
    public GameObject navigationUI; // Set in inspector, includes hamburger and movement UI
    private HamburgerScript hs;
    private GameObject player;
    private GameObject buildingCam;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        buildingCam = GameObject.Find("BuildingCamera");
        //get the animator component
        anim = gameObject.GetComponent<Animator>();
        //disable it on start to stop it from playing the default animation
        anim.enabled = false;
        panelVisible = false;
        hs = navigationUI.transform.Find("HamburgerPanel").GetComponent<HamburgerScript>();
    }

    public void SlideIn() {
        //enable the animator component
        anim.enabled = true;
        //play the Slidein animation
        anim.Play("MapSlideIn");
        //set the isPaused flag to true to indicate that the game is paused
        panelVisible = true;
    }

    public void SlideOut() {
        //set the isPaused flag to false to indicate that the game is not paused
        panelVisible = false;
        //play the SlideOut animation
        anim.Play("MapSlideOut");
    }

    public void TogglePanel() {
        if (panelVisible) {
            if (navigationUI != null) {
                navigationUI.SetActive(true);
                player.GetComponent<FirstPersonController>().canpan = true; // Toggle being able to pan camera in first person mode.
                buildingCam.GetComponent<DragMouseOrbitScript>().canPan = true;
            }
            GetComponent<MapGraphTransitionScript>().GraphOff();
            hs.GetComponent<Animator>().enabled = false;
            SlideOut();
        }
        else {      
            if (navigationUI != null) {
                navigationUI.SetActive(false);
                player.GetComponent<FirstPersonController>().canpan = false;
                buildingCam.GetComponent<DragMouseOrbitScript>().canPan = false;
            }
            SlideIn();
        }
    }
}
