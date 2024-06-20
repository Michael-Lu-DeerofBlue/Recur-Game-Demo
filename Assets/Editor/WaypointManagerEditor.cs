using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
    private SerializedProperty waypointTransformsProperty;
    private SerializedProperty edgesProperty;

    private void OnEnable()
    {
        waypointTransformsProperty = serializedObject.FindProperty("waypointTransforms");
        edgesProperty = serializedObject.FindProperty("edges");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(waypointTransformsProperty, true);

        EditorGUILayout.LabelField("Edges", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Edge"))
        {
            edgesProperty.arraySize++;
            var newEdge = edgesProperty.GetArrayElementAtIndex(edgesProperty.arraySize - 1);
            newEdge.FindPropertyRelative("From").objectReferenceValue = null;
            newEdge.FindPropertyRelative("To").objectReferenceValue = null;
            newEdge.FindPropertyRelative("Cost").floatValue = 0;
        }

        for (int i = 0; i < edgesProperty.arraySize; i++)
        {
            var edge = edgesProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(edge.FindPropertyRelative("From"), GUIContent.none);
            EditorGUILayout.PropertyField(edge.FindPropertyRelative("To"), GUIContent.none);
            EditorGUILayout.PropertyField(edge.FindPropertyRelative("Cost"), GUIContent.none);
            // Calculate cost based on distance
            var fromTransform = edge.FindPropertyRelative("From").objectReferenceValue as Transform;
            var toTransform = edge.FindPropertyRelative("To").objectReferenceValue as Transform;
            if (fromTransform != null && toTransform != null)
            {
                float distance = Vector3.Distance(fromTransform.position, toTransform.position);
                edge.FindPropertyRelative("Cost").floatValue = distance;
            }
            if (GUILayout.Button("Remove"))
            {
                edgesProperty.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
