using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlideScript : MonoBehaviour {
    private Animator anim;
    public bool panelVisible;
    public GameObject navUI;
    private GameObject player;

    // Use this for initialization
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = gameObject.GetComponent<Animator>();
        //disable it on start to stop it from playing the default animation
        anim.enabled = false;
        panelVisible = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && panelVisible) {
            TogglePanel();
        }
    }

    public void SlideIn() {
        //enable the animator component
        anim.enabled = true;
        //play the Slidein animation
        anim.Play("UISlideIn");

        panelVisible = true;
    }

    public void SlideOut() {
        //play the SlideOut animation
        anim.Play("UISlideOut");

        panelVisible = false;
    }

    public void TogglePanel() {
        navUI.SetActive(!navUI.activeSelf);
        if (panelVisible) {
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().canpan = true;
            SlideOut();
        }
        else {
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().canpan = false;
            SlideIn();
        }
    }
}
