using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoronoiDiagram))]
public class VoronoiDiagramEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default convertor

        VoronoiDiagram script = (VoronoiDiagram)target;

        if (GUILayout.Button("Generate Voronoi Texture"))
        {
            script.GenerateVoronoiTexture();
            script.ApplyTexture();
        }

        if (GUILayout.Button("Save Texture"))
        {
            SaveTextureAsAsset(script.GetVoronoiTexture());
        }
    }

    private void SaveTextureAsAsset(Texture2D texture)
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Texture", "VoronoiTexture", "png", "Please enter a file name to save the texture to");
        if (path.Length != 0)
        {
            // Convert the texture to PNG
            byte[] pngData = texture.EncodeToPNG();
            if (pngData != null)
            {
                System.IO.File.WriteAllBytes(path, pngData);
                AssetDatabase.ImportAsset(path); // Refresh the AssetDatabase containing the new asset
                AssetDatabase.Refresh();
                Debug.Log("Texture saved as new asset at " + path);
            }
        }
    }
}