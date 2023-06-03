using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CableView)), CanEditMultipleObjects]
public class CableViewEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        if (GUILayout.Button("Gen"))
        {
            foreach (var c in targets)
            {
                CableView cableView = c as CableView;
                cableView.CalculatePath();
            }
        }
    }
}
