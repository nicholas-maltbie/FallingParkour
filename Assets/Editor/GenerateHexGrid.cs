
using PropHunt.Environment.Hexagon;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RadialHexagonGrid))]
public class ColliderCreatorEditor : Editor
{
    override public void  OnInspectorGUI ()
    {
        RadialHexagonGrid hexagonGrid = (RadialHexagonGrid)target;
        if(GUILayout.Button("Create Grid"))
        {
            hexagonGrid.InitializeGrid();
        }
        DrawDefaultInspector();
    }
}