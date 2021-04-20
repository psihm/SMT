using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {
    public void LoadScene(int Scene)
    {
        SceneManager.LoadScene(Scene);
    }
	
}
