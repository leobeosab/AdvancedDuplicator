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

        this.x = "";
        this.y = "";
        this.z = "";
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
        EditorGUILayout.TextField(x);
        GUILayout.Label("Y");
        EditorGUILayout.TextField(y);
        GUILayout.Label("Z");
        EditorGUILayout.TextField(z);
        EditorGUILayout.EndHorizontal();
        return r;
    }

    public Vector3 getVector3OffsetAtPoint(int n, Vector3 input)
    {
        float xVal = float.Parse(this.x);
        float yVal = float.Parse(this.y);
        float zVal = float.Parse(this.z);

        return new Vector3(xVal + input.x * n, yVal + input.y * n, zVal + input.z * n);
    }
}
