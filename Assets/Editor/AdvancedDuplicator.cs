using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

public class AdvancedDuplicator : EditorWindow
{

    private GameObject selectedObject;

    private DynamicVector3Input objPosition;
    private DynamicVector3Input objRotation;
    private DynamicVector3Input objScale;

    private Transform transform;

    private ArrayList duplicatedObjects;

    [MenuItem("Window/Utilities/AdvancedDuplicator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AdvancedDuplicator));
    }

    private void OnSelectionChange()
    {
        removeDuplicates();
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

    private void duplicate(int amount)
    {
        removeDuplicates();
        duplicatedObjects = new ArrayList();
        
        for (int i = 1; i <= amount; i++)
        {
            Vector3 position = this.objPosition.getVector3OffsetAtPoint(i);
            Vector3 rotation = this.objRotation.getVector3OffsetAtPoint(i);
            Vector3 scale = this.objScale.getVector3OffsetAtPoint(i);

            GameObject newObj = Instantiate(this.selectedObject);

            newObj.transform.position += position;
            newObj.transform.eulerAngles += rotation;
            newObj.transform.localScale += scale;

            duplicatedObjects.Add(
                newObj
               );
        }
    }


    private void OnGUI()
    {
        string objName = this.selectedObject == null || Selection.gameObjects.Length > 1 ? "No object selected" : this.selectedObject.name;

        GUILayout.Label("Duplicate Object: " + objName, EditorStyles.boldLabel);
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

        GUILayout.Label("Amount: ");

        if (GUILayout.Button("Duplicate"))
            this.duplicate(4);
    }

    private void removeDuplicates()
    {
        if (this.duplicatedObjects == null || this.duplicatedObjects.Count == 0)
            return;
        Debug.Log("deleting");
        
        foreach (GameObject dupe in this.duplicatedObjects)
        {
            DestroyImmediate(dupe);
        }

        this.duplicatedObjects.Clear();
    }
}
