#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddMaterialScript))]
public class AssignMaterialsEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        AddMaterialScript ams = (AddMaterialScript)target;
        if (GUILayout.Button("Add Material To Missing")) {
            ams.AddMat();
        }
    }
}
#endif
