using System;
using UnityEngine;
using UnityEditor;

public class AdvancedDuplicator : EditorWindow
{

    private GameObject selectedObject;

    private DynamicVector3Input objPosition;
    private DynamicVector3Input objRotation;
    private DynamicVector3Input objScale;

    private Transform transform;

    [MenuItem("Window/Utilities/AdvancedDuplicator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AdvancedDuplicator));
        
    }

    private void OnSelectionChange()
    {
        SetItemValues();
        Repaint();
    }

    private void SetItemValues()
    {
        this.selectedObject = Selection.activeGameObject;
    }

    private void initializeObjects()
    {
        this.objPosition = new DynamicVector3Input("Position");
        this.objRotation = new DynamicVector3Input("Rotation");
        this.objScale = new DynamicVector3Input("Scale");
    }

    private void SetValuesWithActiveObject()
    {
        this.objPosition.vectorSet = this.selectedObject.transform.position;
        this.objRotation.vectorSet = this.selectedObject.transform.rotation.eulerAngles;
        this.objScale.vectorSet = this.selectedObject.transform.localScale;
    }

    private void duplicate()
    {
        
    }

    private void OnGUI()
    {
        string name = this.selectedObject == null || Selection.gameObjects.Length > 1 ? "No object selected" : this.selectedObject.name;

        GUILayout.Label("Duplicate Object: " + name, EditorStyles.boldLabel);
        if (this.selectedObject == null || Selection.gameObjects.Length > 1)
            return;
        if (this.objPosition == null)
        {
            initializeObjects();
            SetItemValues();
        }

        GUILayout.Label("Offsets: ");
        this.objPosition.drawGUI();
        this.objRotation.drawGUI();
        this.objScale.drawGUI();

        if (GUILayout.Button("Duplicate"))
            duplicate();
    }

}
