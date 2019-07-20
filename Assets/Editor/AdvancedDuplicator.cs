// Suppress over cautious warning
#pragma warning disable RECS0117 // Local variable has the same name as a member and hides it

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

    private enum DUPE_TYPE {
        LINEAR,
        RECT,
        CUBE
    }

    private DUPE_TYPE type = DUPE_TYPE.LINEAR;
    private DUPE_TYPE oldType = 0;

    private GameObject selectedObject;

    private DynamicVector3Input objPosition;
    private DynamicVector3Input objRotation;
    private DynamicVector3Input objScale;

    private int width = 0, length = 0, height = 0, numberOfDupes = 0;

    private readonly string previewNameSuffix = "[~Preview~]";

    private Transform transform;

    private ArrayList previewObjects;

    private string oldPositionString;
    private string oldRotationString;
    private string oldScaleString;
    private int oldWidth, oldLength, oldHeight, oldNumberOfDupes;

    private Hashtable sceneObjectSettings;

    [MenuItem("Window/Utilities/Advanced Duplicator")]
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
        this.objPosition = new DynamicVector3Input("Position", this.selectedObject);
        this.objRotation = new DynamicVector3Input("Rotation", this.selectedObject);
        this.objScale = new DynamicVector3Input("Scale", this.selectedObject);
    }

    private void SetValuesWithActiveObject()
    {
        this.objPosition.vectorSet = this.selectedObject.transform.position;
        this.objRotation.vectorSet = this.selectedObject.transform.rotation.eulerAngles;
        this.objScale.vectorSet = this.selectedObject.transform.localScale;

        Vector3 objSize = this.selectedObject.GetComponent<MeshFilter>().sharedMesh.bounds.size;

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
            oldWidth = this.width;
            oldHeight = this.height;
            oldLength = this.length;
            oldNumberOfDupes = this.numberOfDupes;

            // There is a shit ton of repeated code in this
            // TODO: Refactor this POS
            switch (this.type)
            {
                case DUPE_TYPE.LINEAR:

                    for (int i = 1; i <= numberOfDupes; i++)
                    {
                        Vector3 position = this.objPosition.getVector3OffsetAtPoint(i);
                        Vector3 rotation = this.objRotation.getVector3OffsetAtPoint(i);
                        Vector3 scale = this.objScale.getVector3OffsetAtPoint(i);

                        GameObject newObj = Instantiate(this.selectedObject, this.selectedObject.transform.parent);

                        newObj.transform.position += position;
                        newObj.transform.eulerAngles += rotation;
                        newObj.transform.localScale += scale;

                        newObj.name = previewNameSuffix;

                        previewObjects.Add(newObj);
                    }
                    break;
                case DUPE_TYPE.RECT:
                    for (int l = 0; l < this.length; l++)
                    {
                        for (int w = 0; w < this.width; w++)
                        {
                            if (l == 0 && w == 0)
                                continue;

                            // This is really bad TODO fix asap
                            Vector3 position = this.objPosition.getVector3OffsetAtPoint(l);
                            Vector3 xPos = this.objPosition.getVector3OffsetAtPoint(w);
                            Vector3 rotation = this.objRotation.getVector3OffsetAtPoint(l);
                            Vector3 scale = this.objScale.getVector3OffsetAtPoint(l);

                            position.x = xPos.x;

                            position += new Vector3(w * this.objPosition.width, 0, l * this.objPosition.length);

                            GameObject newObj = Instantiate(this.selectedObject, this.selectedObject.transform.parent);

                            newObj.transform.position += position;
                            newObj.transform.eulerAngles += rotation;
                            newObj.transform.localScale += scale;

                            newObj.name = previewNameSuffix;

                            previewObjects.Add(newObj);
                        }
                    }
                    break;
                case DUPE_TYPE.CUBE:
                    for (int h = 0; h < this.height; h++)
                    {
                        for (int l = 0; l < this.length; l++)
                        {
                            for (int w = 0; w < this.width; w++)
                            {
                                if (h == 0 && l == 0 && w == 0)
                                    continue;

                                // Also really bad TODO fix asap
                                Vector3 position = this.objPosition.getVector3OffsetAtPoint(l);
                                Vector3 xPos = this.objPosition.getVector3OffsetAtPoint(w);
                                Vector3 yPos = this.objPosition.getVector3OffsetAtPoint(h);

                                position.x = xPos.x;
                                position.y = yPos.y;

                                Vector3 rotation = this.objRotation.getVector3OffsetAtPoint(l);
                                Vector3 scale = this.objScale.getVector3OffsetAtPoint(l);

                                position += new Vector3(w * this.objPosition.width, h * this.objPosition.height, l * this.objPosition.length);

                                GameObject newObj = Instantiate(this.selectedObject);

                                newObj.transform.position += position;
                                newObj.transform.eulerAngles += rotation;
                                newObj.transform.localScale += scale;

                                newObj.name = previewNameSuffix;

                                previewObjects.Add(newObj);
                            }
                        }
                    }
                    break;
            }

        }
        catch (Exception)
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
        this.numberOfDupes = 0;
    }

    private bool HasChanged()
    {
        if (!this.objPosition.ToString().Equals(oldPositionString) ||
            !this.objRotation.ToString().Equals(oldRotationString) ||
            !this.objScale.ToString().Equals(oldScaleString) ||
            this.numberOfDupes != this.oldNumberOfDupes ||
            this.width != this.oldWidth ||
            this.length != this.oldLength ||
            this.height != this.oldHeight ||
            this.oldType != this.type)
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

        // Styles
        GUIStyle buttonStyle = EditorStyles.miniButton;
        buttonStyle.padding = new RectOffset(25, 25, 5, 5);
        buttonStyle.fontSize = 12;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle verticalStyle = new GUIStyle();
        verticalStyle.padding = new RectOffset(10, 10, 5, 5);

        // Start GUI
        GUILayout.BeginVertical(verticalStyle);
        GUILayout.Space(10);

        // Duplication Button Types
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        
        this.oldType = this.type;
        this.type = (DUPE_TYPE) GUILayout.Toolbar((int) this.type, new String[] { "Linear", "Rect", "Cube" }, GUILayout.Height(30));

        GUILayout.Space(10);
        GUILayout.EndHorizontal();
        // End

        GUILayout.Space(10);

        this.DrawTypeSpecificInputs();

        GUILayout.Space(10);

        // Offsets
        GUILayout.Label("Offsets: ");
        this.objPosition.drawGUI();
        this.objRotation.drawGUI();
        this.objScale.drawGUI();
        // End

        GUILayout.Space(20);

        // End

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Duplicate / Reset buttons
        if (GUILayout.Button("Reset", buttonStyle, GUILayout.Height(30)))
        {
            this.InitializeObjects();
            this.width = 0;
            this.height = 0;
            this.length = 0;
            this.numberOfDupes = 0;
        }
        if (GUILayout.Button("Duplicate", buttonStyle, GUILayout.Height(30)))
        {
            this.Duplicate();
        }
        // End

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.EndVertical();
        // End GUI
    }

    private void DrawTypeSpecificInputs()
    {
        switch (this.type)
        {
            case DUPE_TYPE.LINEAR:
                // Amount
                Rect r = new Rect(0, 0, 100, 100);
                numberOfDupes = EditorGUILayout.IntSlider("Duplicate Amount: ", numberOfDupes, 0, 100);
                break;
            case DUPE_TYPE.RECT:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Duplication Size");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Width");
                this.width = EditorGUILayout.IntField(width);
                GUILayout.Label("Length");
                this.length = EditorGUILayout.IntField(length);
                GUILayout.EndHorizontal();
                break;
            case DUPE_TYPE.CUBE:
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Width");
                this.width = EditorGUILayout.IntField(width);
                GUILayout.Label("Length");
                this.length = EditorGUILayout.IntField(length);
                GUILayout.Label("Height");
                this.height = EditorGUILayout.IntField(height);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                break;

        }
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
