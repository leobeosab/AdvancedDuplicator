using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Data;
using System;

public class DynamicVector3Input
{
    // Name of input
    private string name;

    private string x;
    private string y;
    private string z;

    public float width;
    public float height;
    public float length;

    public Vector3 vectorSet 
    {
        set
        {
            this.x = value.x.ToString();
            this.y = value.y.ToString();
            this.z = value.z.ToString();
        }
    }

 
    public DynamicVector3Input(string name, GameObject obj)
    {
        this.name = name;

        this.x = "0";
        this.y = "0";
        this.z = "0";

        // Fails if game object doesn't have a mesh renderer
        try
        {
            Vector3 objSize = obj.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            Vector3 objScale = obj.transform.localScale;

            this.width = objSize.x * objScale.x;
            this.length = objSize.z * objScale.z;
            this.height = objSize.y * objScale.y;
        } catch (Exception)
        {
            this.width = this.length = this.height = 0;
        }
    }

    public Rect drawGUI()
    {
        Rect r = EditorGUILayout.BeginHorizontal();
        GUILayout.Label(this.name, 
            GUILayout.MaxWidth(180),
            GUILayout.MinWidth(80));
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        GUILayout.Label("X");
        x = EditorGUILayout.TextField(x); // Set the textfield to X and set X to be updated by the field
        GUILayout.Label("Y");
        y = EditorGUILayout.TextField(y);
        GUILayout.Label("Z");
        z = EditorGUILayout.TextField(z);

        EditorGUILayout.EndHorizontal();
        return r;
    }

    public Vector3 getVector3OffsetAtPoint(int n)
    {
        DataTable dt = new DataTable();

        string xString = InsertValues(this.x, n);
        string yString = InsertValues(this.y, n);
        string zString = InsertValues(this.z, n);

        float xVal = Evaluate(xString);
        float yVal = Evaluate(yString);
        float zVal = Evaluate(zString);

        return new Vector3(xVal * n, yVal * n, zVal * n);
    }

    public override string ToString()
    {
        return x + y + z;
    }

    private string InsertValues(string input, int n)
    {
        input = input.ToLower();

        string output = input.Replace("n", n.ToString());
        output = output.Replace("w", width.ToString());
        output = output.Replace("h", height.ToString());

        return output;
    }

    private float Evaluate(string expression)
    {
        var loDataTable = new DataTable();
        var loDataColumn = new DataColumn("Eval", typeof(double), expression);
        loDataTable.Columns.Add(loDataColumn);
        loDataTable.Rows.Add(0);
        return (float)(double)(loDataTable.Rows[0]["Eval"]);
    }
}
