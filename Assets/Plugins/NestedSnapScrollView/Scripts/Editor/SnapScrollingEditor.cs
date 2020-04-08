using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SnapScrolling))]
public class SnapScrollingEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("UpdateScroll"))
        {
            SnapScrolling t = (SnapScrolling)target;
            t.UpdateScroll();
        }
    }
}
