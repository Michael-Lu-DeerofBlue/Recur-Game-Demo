#if UNITY_EDITOR && !ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Kamgam.UGUIComponentsForSettings
{
    public static class InputAxisButtonEventsEditor
    {
        [InitializeOnLoadMethod]
        static void addOldInputAxis()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            // A crude way to test for the existence of an axis.
            bool axisExists = false;
            try
            {
                // Some foobar action to trigger Input.GetAxis() code.
                // If the axis exists then this always evaluates to true at runtime.
                if (Input.GetAxis(InputAxisButtonEvents.OldInputAxisX) > -999000999f)
                {
                    axisExists = true;
                }
            }
            catch
            {
                axisExists = false;
            }

            if (!axisExists)
            {
                addMissingAxis();
                Debug.Log("InputAxisButtonEventsEditor added some 'KamgamInput...' axis to the InputManager. Hope that's okay.");
            }
        }

        [MenuItem("Tools/Settings Generator/Debug/Add axis to old InputSystem")]
        static void addMissingAxis()
        {
            var inputManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            var serializedObject = new SerializedObject(inputManagerAsset);
            var axisArray = serializedObject.FindProperty("m_Axes");

            var requiredAxis = new string[] { 
                InputAxisButtonEvents.OldInputAxisX, 
                InputAxisButtonEvents.OldInputAxisY, 
                InputAxisButtonEvents.OldInputAxis4, 
                InputAxisButtonEvents.OldInputAxis5, 
                InputAxisButtonEvents.OldInputAxis6, 
                InputAxisButtonEvents.OldInputAxis7,
                InputAxisButtonEvents.OldInputAxis9,
                InputAxisButtonEvents.OldInputAxis10 
            };
            var newAxis = new List<string>();

            foreach (var newAxisName in requiredAxis)
            {
                bool found = false;
                for (var i = 0; i < axisArray.arraySize; i++)
                {
                    var axisEntry = axisArray.GetArrayElementAtIndex(i);
                    var axisName = getProperty(axisEntry, "m_Name").stringValue;
                    if (axisName == newAxisName)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    newAxis.Add(newAxisName);
                }
            }

            int newIndex = axisArray.arraySize;
            axisArray.arraySize = axisArray.arraySize + newAxis.Count;
            serializedObject.ApplyModifiedProperties();
            axisArray = serializedObject.FindProperty("m_Axes");

            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxisX)) 
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex), 
                    name: InputAxisButtonEvents.OldInputAxisX,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 0,
                    joyNum: 1
                    );
                newIndex++;
            }
            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxisY))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxisY,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 1,
                    joyNum: 1
                    );
                newIndex++;
            }
            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis4))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis4,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 3,
                    joyNum: 1
                    );
                newIndex++;
            }
            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis5))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis5,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 4,
                    joyNum: 1
                    );
                newIndex++;
            }
            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis6))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis6,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 5,
                    joyNum: 1
                    );
                newIndex++;
            }
            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis7))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis7,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 6,
                    joyNum: 1
                    );
                newIndex++;
            }

            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis9))
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis9,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 8,
                    joyNum: 1
                    );
                newIndex++;
            }

            if (newAxis.Contains(InputAxisButtonEvents.OldInputAxis10)) 
            {
                addSingleAxis(
                    axisArray.GetArrayElementAtIndex(newIndex),
                    name: InputAxisButtonEvents.OldInputAxis10,
                    descriptiveName: "",
                    descriptiveNegativeName: "",
                    negativeButton: "",
                    positiveButton: "",
                    altNegativeButton: "",
                    altPositiveButton: "",
                    gravity: 0f,
                    deadZone: 0.19f,
                    sensitivity: 1.0f,
                    snap: false,
                    invert: false,
                    type: 2,
                    axis: 9,
                    joyNum: 1
                    );
                // newIndex++;
            }

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh();
        }

        static void addSingleAxis(
            SerializedProperty axisEntry,
            string name,
            string descriptiveName,
            string descriptiveNegativeName,
            string negativeButton,
            string positiveButton,
            string altNegativeButton,
            string altPositiveButton,
            float gravity,
            float deadZone,
            float sensitivity,
            bool snap,
            bool invert,
            int type,
            int axis,
            int joyNum
            )
        {
            getProperty(axisEntry, "m_Name").stringValue = name;
            getProperty(axisEntry, "descriptiveName").stringValue = descriptiveName;
            getProperty(axisEntry, "descriptiveNegativeName").stringValue = descriptiveNegativeName;
            getProperty(axisEntry, "negativeButton").stringValue = negativeButton;
            getProperty(axisEntry, "positiveButton").stringValue = positiveButton;
            getProperty(axisEntry, "altNegativeButton").stringValue = altNegativeButton;
            getProperty(axisEntry, "altPositiveButton").stringValue = altPositiveButton;
            getProperty(axisEntry, "gravity").floatValue = gravity;
            getProperty(axisEntry, "dead").floatValue = deadZone;
            getProperty(axisEntry, "sensitivity").floatValue = sensitivity;
            getProperty(axisEntry, "snap").boolValue = snap;
            getProperty(axisEntry, "invert").boolValue = invert;
            getProperty(axisEntry, "type").intValue = type;
            getProperty(axisEntry, "axis").intValue = axis;
            getProperty(axisEntry, "joyNum").intValue = joyNum;

        }

        static SerializedProperty getProperty(SerializedProperty parent, string name)
        {
            var child = parent.Copy();
            child.Next(true);

            do
            {
                if (child.name == name)
                {
                    return child;
                }
            } while (child.Next(false));

            return null;
        }
    }
}
#endif