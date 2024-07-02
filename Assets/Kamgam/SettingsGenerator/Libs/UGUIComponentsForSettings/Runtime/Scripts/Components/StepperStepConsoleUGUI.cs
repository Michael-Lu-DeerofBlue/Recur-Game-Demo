using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Kamgam.UGUIComponentsForSettings
{
    public class StepperStepConsoleUGUI : MonoBehaviour
    {
        public static List<StepperStepConsoleUGUI> CreateSteps(Transform container, GameObject template, int totalSteps)
        {
            if (template == null)
                return null;

#if UNITY_EDITOR
            // Don't add instances if in prefab stage.
            if (isInPrefabStage())
                return null;
#endif

            template.gameObject.SetActive(false);
#pragma warning disable CS0219
            bool templateIsPrefab = false;
#pragma warning restore CS0219
#if UNITY_EDITOR
            templateIsPrefab = UnityEditor.PrefabUtility.GetPrefabAssetType(template) != UnityEditor.PrefabAssetType.NotAPrefab;

            if (templateIsPrefab)
            {
                var prefabRoot = UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(template);
                if (UnityEditor.PrefabUtility.GetPrefabInstanceStatus(prefabRoot) != UnityEditor.PrefabInstanceStatus.Connected)
                {
                    return null;
                }
            }
#endif

            var foundSteps = container.GetComponentsInChildren<StepperStepConsoleUGUI>(includeInactive: true);
            foreach (var step in foundSteps)
            {
                if (!step.name.EndsWith("_Template"))
                {
                    smartDestroy(step.gameObject);
                }
            }

            var steps = new List<StepperStepConsoleUGUI>();
            for (int i = 0; i < totalSteps; i++)
            {

                GameObject go;
#if UNITY_EDITOR
                if (templateIsPrefab)
                {
                    // TODO: n2h would be this solution with a reflection check before:
                    // Thansk to: https://forum.unity.com/threads/solved-duplicate-prefab-issue.778553/
                    // var previousSelection = UnityEditor.Selection.objects;
                    // UnityEditor.Selection.activeGameObject = template;
                    // UnityEditor.Unsupported.DuplicateGameObjectsUsingPasteboard();
                    // go = UnityEditor.Selection.activeGameObject;
                    // UnityEditor.Selection.objects = previousSelection;

                    var prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(template);
                    if (prefab)
                    {
                        go = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, container) as GameObject;
                        UnityEditor.PrefabUtility.SetPropertyModifications(go, UnityEditor.PrefabUtility.GetPropertyModifications(template));
                    }
                    else
                    {
                        go = GameObject.Instantiate(template, container);
                    }
                }
                else
                    go = GameObject.Instantiate(template, container);
#else
                go = GameObject.Instantiate(template, container);
#endif
                go.name = template.name.Replace("_Template", "");
                go.SetActive(true);
                var step = go.GetComponent<StepperStepConsoleUGUI>();
                step.SetActive(false);
                steps.Add(step);
            }

            return steps;
        }

#if UNITY_EDITOR
        static bool isInPrefabStage()
        {
#if UNITY_2021_2_OR_NEWER
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#endif
        }
#endif

        public static void SetActive(List<StepperStepConsoleUGUI> steps, int activeSteps)
        {
            if (steps == null)
                return;

#if UNITY_EDITOR
            // Clear out old steps
            for (int i = steps.Count-1; i >= 0; i--)
            {
                if (steps[i] == null || steps[i].gameObject == null)
                {
                    steps.RemoveAt(i);
                }
            }
#endif

            for (int i = 0; i < steps.Count; i++)
            {
                steps[i].SetActive(i < activeSteps);
            }
        }

        static void smartDestroy(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
#endif
            {
                GameObject.Destroy(obj);
            }
        }

        public GameObject Inactive;
        public GameObject Active;

        public void SetActive(bool active)
        {
            Active.gameObject.SetActive(active);
            Inactive.gameObject.SetActive(!active);
        }
    }
}
