using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomOutline))]
public class OutlineScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CustomOutline outline = (CustomOutline)target;
        outline.preRenderOutline = EditorGUILayout.Toggle("Prerender Outline",outline.preRenderOutline);
        outline.activeOutline = EditorGUILayout.Toggle("Activate Outline",outline.activeOutline);

        outline.outlineColor = EditorGUILayout.ColorField(new GUIContent("Outline Color"), outline.outlineColor, true, true, true, new ColorPickerHDRConfig(0, 1, 0, 3), GUILayout.MaxWidth(100f));
        outline.outlineWidth = EditorGUILayout.Slider("Outline Width",outline.outlineWidth,0,20);
        outline.seeThroughWalls = EditorGUILayout.Toggle("See Outlines through Walls",outline.seeThroughWalls);
        
        if (GUILayout.Button("Recalculate Normals"))
            outline.RecalculateNormals();

        outline.activeCutoff = EditorGUILayout.Toggle("Activate Cutoff Texture",outline.activeCutoff);
        if (outline.activeCutoff)
            outline.cutoffTex = (Texture2D)EditorGUILayout.ObjectField("Cutoff Texture",outline.cutoffTex,typeof(Texture2D));
    }
}
