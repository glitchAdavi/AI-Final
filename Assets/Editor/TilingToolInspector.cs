using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Boo.Lang.Environments;

[CustomEditor(typeof(TilingTool))]
public class TilingToolInspector : Editor
{
    TilingTool myTool;
    Tool lastTool = Tool.None;

    private void OnEnable()
    {
        myTool = target as TilingTool;

        lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void OnDisable()
    {
        Tools.current = lastTool;
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.white;

        Handles.BeginGUI();
        var sceneViewHeight = EditorWindow.GetWindow<SceneView>().camera.scaledPixelHeight;
        var sceneViewWidth = EditorWindow.GetWindow<SceneView>().camera.scaledPixelWidth;

        GUILayout.BeginArea(new Rect(sceneViewWidth - 300, sceneViewHeight - 275, 300, 900));

        var r = EditorGUILayout.BeginVertical();

        GUI.color = new Color(0.6f, 0.6f, 0.6f);
        GUI.Box(r, GUIContent.none);
        GUILayout.Space(5);

        myTool.objectToTile = EditorGUILayout.ObjectField("Object to Tile: ", myTool.objectToTile, typeof(GameObject), true) as GameObject;

        GUILayout.Space(5);

        EditorGUI.BeginDisabledGroup(myTool.objectToTile == null);
        myTool.startPos = EditorGUILayout.Vector3Field("Start Pos: ", myTool.startPos);

        GUILayout.Space(5);

        myTool.xNum = EditorGUILayout.IntField("X Size: ", myTool.xNum);
        if (myTool.xNum < 1) myTool.xNum = 1;
        myTool.yNum = EditorGUILayout.IntField("Y Size: ", myTool.yNum);
        if (myTool.yNum < 1) myTool.yNum = 1;
        myTool.zNum = EditorGUILayout.IntField("Z Size: ", myTool.zNum);
        if (myTool.zNum < 1) myTool.zNum = 1;

        GUILayout.Space(5);

        myTool.xSpace = EditorGUILayout.FloatField("X Space: ", myTool.xSpace);
        if (myTool.xSpace < 0) myTool.xSpace = 0;
        myTool.ySpace = EditorGUILayout.FloatField("Y Space: ", myTool.ySpace);
        if (myTool.ySpace < 0) myTool.ySpace = 0;
        myTool.zSpace = EditorGUILayout.FloatField("Z Space: ", myTool.zSpace);
        if (myTool.zSpace < 0) myTool.zSpace = 0;

        GUILayout.Space(15);

        if (GUI.Button(GUILayoutUtility.GetRect(20, 20, 20, 20), "Place"))
        {
            myTool.PlaceObjects();
        }
        if (GUI.Button(GUILayoutUtility.GetRect(20, 20, 20, 20), "Confirm"))
        {
            myTool.ConfirmObjects();
        }

        GUILayout.Space(15);

        if (GUI.Button(GUILayoutUtility.GetRect(20, 20, 20, 20), "Remove"))
        {
            myTool.RemoveObjects();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}
