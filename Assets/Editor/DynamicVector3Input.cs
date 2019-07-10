using UnityEngine;
using UnityEditor;
using System.Collections;

public class DynamicVector3Input
{
    // Name of input
    private string name;

    private string x;
    private string y;
    private string z;

    public Vector3 vectorSet 
    {
        set
        {
            this.x = value.x.ToString();
            this.y = value.y.ToString();
            this.z = value.z.ToString();
        }
    }

 
    public DynamicVector3Input(string name)
    {
        this.name = name;

        this.x = "0";
        this.y = "0";
        this.z = "0";
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
        float xVal = float.Parse(this.x);
        float yVal = float.Parse(this.y);
        float zVal = float.Parse(this.z);

        return new Vector3(xVal * n, yVal * n, zVal * n);
    }

    public override string ToString()
    {
        return x + y + z;
    }
}
