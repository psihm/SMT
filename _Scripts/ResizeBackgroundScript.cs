using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeBackgroundScript : MonoBehaviour {
    // Use this for initialization
    [SerializeField]
    private float bgXFloat = 1.3f;
    [SerializeField]
    private float bgYFloat = 2.0f;
    IEnumerator Start () {
        RectTransform uiTextRect = GetComponentInChildren<Text>().gameObject.GetComponent<RectTransform>();
        uiTextRect.GetComponent<Text>().text = transform.parent.parent.name;
        yield return null;
        yield return new WaitForEndOfFrame();
        RectTransform uiImage = GetComponent<RectTransform>();
        uiImage.sizeDelta = new Vector2(uiTextRect.sizeDelta.x * bgXFloat * uiTextRect.localScale.x, uiTextRect.sizeDelta.y * (bgYFloat + 0.5f) * uiTextRect.localScale.y);
    }
}
