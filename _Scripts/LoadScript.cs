using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour {

    private bool loadScene = false;
    
    private string scene = "Red River STTC";
    [SerializeField]
    private Text loadingText;

    void Awake() {
        loadingText = GetComponent<Text>();
        loadingText.text = "Click to Load";
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && !loadScene) {
            loadScene = true;
            loadingText.text = "Loading...";
            StartCoroutine(LoadNewScene());         
        }
        if (loadScene)
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }

    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene() {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
            yield return null;
        }
    }

}