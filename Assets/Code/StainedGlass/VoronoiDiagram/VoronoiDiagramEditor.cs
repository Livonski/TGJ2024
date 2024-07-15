using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoronoiDiagram))]
public class VoronoiDiagramEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoronoiDiagram voronoiDiagram = (VoronoiDiagram)target;

        if (GUILayout.Button("Generate Voronoi Texture"))
        {
            voronoiDiagram.GenerateVoronoiTexture();
            voronoiDiagram.ApplyTexture();
        }
    }
}