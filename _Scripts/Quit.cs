using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour
{
    public void LogoutQuit() {
        SmtApiScript[] apiarray = GetComponents<SmtApiScript>();
        foreach (SmtApiScript api in apiarray) {
            api.LogoutWrapper();
        }
        Application.Quit();
    }
}