using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EulerToSpherical))]
public class EulerToSphericalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target script
        EulerToSpherical eulerToSpherical = (EulerToSpherical)target;

        // Calculate spherical coordinates
        if (GUILayout.Button("Update Spherical Coordinates"))
        {
            eulerToSpherical.UpdateSphericalCoordinates();
        }

        // Display spherical coordinates
        EditorGUILayout.LabelField("Phi (rad)", eulerToSpherical.phi.ToString());
        EditorGUILayout.LabelField("Theta (rad)", eulerToSpherical.theta.ToString());
    }
}
