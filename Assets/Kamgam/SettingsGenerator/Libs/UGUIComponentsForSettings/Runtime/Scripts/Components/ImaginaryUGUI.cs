using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UGUIComponentsForSettings
{
    public class ImaginaryUGUI : MaskableGraphic
    {
        [Tooltip("Enable to change to a circular hit area.")]
        public bool Circular = false;
        public float Radius = 0;

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            bool result = base.Raycast(sp, eventCamera);
            if (Circular && result)
            {
                Vector2 localRectPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out localRectPoint);
                float radius = Radius;
                if (Radius <= 0)
                {
                    radius = Mathf.Min(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f);
                }
                return localRectPoint.sqrMagnitude < radius * radius;
            }
            return result;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Circular)
            {
                Gizmos.color = new Color(0.7f, 1f, 0.7f);
                Gizmos.matrix = canvas.transform.localToWorldMatrix;
                if (Radius > 0)
                {
                    Gizmos.DrawWireSphere(rectTransform.localPosition, Radius);
                }
                else
                {
                    Gizmos.DrawWireSphere(rectTransform.localPosition, Mathf.Min(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f));
                }
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ImaginaryUGUI))]
    public class ImaginaryEditor : Editor
    {
        private SerializedProperty m_RacastTarget;
        private SerializedProperty m_Circular;
        private SerializedProperty m_Radius;

        void OnEnable()
        {
            m_RacastTarget = serializedObject.FindProperty("m_RaycastTarget");
            m_Circular = serializedObject.FindProperty("Circular");
            m_Radius = serializedObject.FindProperty("Radius");
        }

        public override void OnInspectorGUI()
        {
            var imaginary = target as ImaginaryUGUI;
            EditorGUILayout.PropertyField(m_RacastTarget, new GUIContent("Raycast Target"));
            if (imaginary.raycastTarget)
            {
                EditorGUILayout.PropertyField(m_Circular, new GUIContent("Circular"));
                if (imaginary.Circular)
                {
                    EditorGUILayout.PropertyField(m_Radius, new GUIContent("Radius"));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
