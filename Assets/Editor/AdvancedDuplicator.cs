using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

public class AdvancedDuplicator : EditorWindow
{

    struct DynamicTransform
    {
        public DynamicVector3Input position, rotation, scale;
        public int count;

        public DynamicTransform(DynamicVector3Input p, DynamicVector3Input r, DynamicVector3Input s, int c)
        {
            position = p;
            rotation = r;
            scale = s;
            count = c;
        }
    }

    private GameObject selectedObject;

    private DynamicVector3Input objPosition;
    private DynamicVector3Input objRotation;
    private DynamicVector3Input objScale;

    private int numberOfDupes = 0;
    private readonly string previewNameSuffix = "[~Preview~]";

    private Transform transform;

    private ArrayList previewObjects;

    private string oldPositionString;
    private string oldRotationString;
    private string oldScaleString;
    private int oldNumberOfDupes;

    private Hashtable sceneObjectSettings;

    [MenuItem("Window/Utilities/AdvancedDuplicator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AdvancedDuplicator));
    }

    private void OnSelectionChange()
    {
        RemoveDuplicatesPreview();
        SetItemValues();
        if (this.selectedObject != null)
        {
            if (this.sceneObjectSettings.Contains(this.selectedObject))
            {
                DynamicTransform dynamicTransform = (DynamicTransform)this.sceneObjectSettings[this.selectedObject];
                this.objPosition = dynamicTransform.position;
                this.objRotation = dynamicTransform.rotation;
                this.objScale = dynamicTransform.rotation;
                this.numberOfDupes = dynamicTransform.count;

                this.PreviewDuplicate();
            }
            else
            {
                InitializeObjects();
                this.numberOfDupes = 0;
            }

        }

        Repaint();
    }

    private void SetItemValues()
    {
        this.selectedObject = Selection.activeGameObject;
    }

    private void InitializeObjects()
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

    // Generate the game objects for the preview
    // TODO: make them transparent
    private void PreviewDuplicate()
    {
        try
        {
            RemoveDuplicatesPreview();
            previewObjects = new ArrayList();

            // Used for generating new previews
            oldPositionString = this.objPosition.ToString();
            oldRotationString = this.objRotation.ToString();
            oldScaleString = this.objScale.ToString();
            oldNumberOfDupes = this.numberOfDupes;

            for (int i = 1; i <= numberOfDupes; i++)
            {
                Vector3 position = this.objPosition.getVector3OffsetAtPoint(i);
                Vector3 rotation = this.objRotation.getVector3OffsetAtPoint(i);
                Vector3 scale = this.objScale.getVector3OffsetAtPoint(i);

                GameObject newObj = Instantiate(this.selectedObject);

                newObj.transform.position += position;
                newObj.transform.eulerAngles += rotation;
                newObj.transform.localScale += scale;

                newObj.name = previewNameSuffix;

                previewObjects.Add(newObj);
            }
        } catch (Exception e)
        {
            return;
        }
    }

    private void Duplicate()
    {
        int undoGroupIndex = Undo.GetCurrentGroup();

        PreviewDuplicate();
        foreach (GameObject obj in this.previewObjects)
        {
            obj.name = obj.name.Replace(previewNameSuffix, this.selectedObject.name);
            Undo.RegisterCreatedObjectUndo(obj, "Duped "+ obj +" via AdvancedDuplicator!");
        }
        previewObjects.Clear();
    }

    private bool HasChanged()
    {
        if (!this.objPosition.ToString().Equals(oldPositionString) ||
            !this.objRotation.ToString().Equals(oldRotationString) ||
            !this.objScale.ToString().Equals(oldScaleString) ||
            this.numberOfDupes != this.oldNumberOfDupes)
            return true;
        else
            return false;
    }

    private void OnEnable()
    {
        // Make sure to set HashTable
        if (this.sceneObjectSettings == null)
            this.sceneObjectSettings = new Hashtable();

        // Set Dynamic Inputs
        if (this.objPosition == null)
            this.InitializeObjects();
    }

    private void OnGUI()
    {

        string objName = this.selectedObject == null || Selection.gameObjects.Length > 1 ? "No object selected" : this.selectedObject.name;

        GUILayout.Label("Duplicate Object: " + objName, EditorStyles.boldLabel);
        if (this.selectedObject == null || Selection.gameObjects.Length > 1)
            return;

        // Check state of certain objects
        if (this.objPosition == null)
        {
            SetItemValues();
        }

        if (HasChanged() || this.previewObjects == null)
        {
            if (this.selectedObject != null)
            {
                this.sceneObjectSettings[this.selectedObject] = new DynamicTransform(
                    this.objPosition,
                    this.objRotation,
                    this.objScale,
                    this.numberOfDupes);
            }

            PreviewDuplicate();
        }

        // Offsets
        GUILayout.Label("Offsets: ");
        this.objPosition.drawGUI();
        this.objRotation.drawGUI();
        this.objScale.drawGUI();

        // Amount
        Rect r = new Rect(0, 0, 100, 100);
        numberOfDupes = EditorGUILayout.IntSlider("Amount: ", numberOfDupes, 0, 100);

        // Duplicate Buttons
        if (GUILayout.Button("Duplicate"))
            this.Duplicate();
    }

    private void RemoveDuplicatesPreview()
    {
        if (this.previewObjects == null || this.previewObjects.Count == 0)
            return;

        GameObject tmpObj = GameObject.Find(previewNameSuffix);
        while(tmpObj != null)
        {
            DestroyImmediate(tmpObj);
            tmpObj = GameObject.Find(previewNameSuffix);
        }

        this.previewObjects.Clear();
    }
}
