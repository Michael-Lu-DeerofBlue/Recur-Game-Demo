using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamgam.SettingsGenerator
{
    public class LightDetector
    {
        /// <summary>
        /// If enabled then whenever a new scene is loaded a search will
        /// be done on the whole scene to find new light sources.<br />
        /// If turned off then lights need to be added manually to receive
        /// setting changes (use LightDetectorComponent).
        /// </summary>
        public static bool ScanAfterSceneLoad = true;

        public delegate void OnNewLightFoundDelegate(Light light);

        /// <summary>
        /// Called if a new light source was found.
        /// </summary>
        public OnNewLightFoundDelegate OnNewLightFound;

        private static LightDetector _instance;
        public static LightDetector Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene (actually this class is no MonoBehaviour,
                    // so no danger there. Though we keep this code in to prevent us
                    // from using it during EditMode).
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif

                    _instance = new LightDetector();
                }
                return _instance;
            }
        }

        protected List<Light> _lights = new List<Light>(20);
        /// <summary>
        /// A list of known lights.<br />
        /// NOTICE: The list may contain null values for deleted lights.
        /// </summary>
        public List<Light> Lights
        {
            get => _lights;
        }

        private LightDetector()
        {
            if (ScanAfterSceneLoad)
            {
                ScanAllScenes();
            }

            SceneManager.sceneLoaded += onSceneLoaded;
        }

        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (ScanAfterSceneLoad)
            {
                ScanScene(scene);
            }
        }

        public Light GetPrimaryLight()
        {
            if (_lights.Count > 0)
            {
                foreach (var light in _lights)
                {
                    if (light == null)
                        continue;

                    if (light.isActiveAndEnabled && light.gameObject.activeInHierarchy && light.type == LightType.Directional)
                    {
                        return light;
                    }
                }

                foreach (var light in _lights)
                {
                    if (light == null)
                        continue;

                    if (light.isActiveAndEnabled && light.gameObject.activeInHierarchy)
                    {
                        return light;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the light (if not yet added).
        /// </summary>
        /// <param name="light"></param>
        public void Add(Light light)
        {
            if (light == null)
                return;

            if (!_lights.Contains(light))
            {
                _lights.Add(light);
                OnNewLightFound?.Invoke(light);
            }
        }

        public void ScanAllScenes()
        {
            int scenes = SceneManager.sceneCount;
            for (int i = 0; i < scenes; i++)
            {
                ScanScene(SceneManager.GetSceneAt(i)); 
            }
        }

        private List<GameObject> _tmpRootGameObjects = new List<GameObject>(20);
        private List<Light> _tmpLights = new List<Light>(20);

        public void ScanActiveScene()
        {
            ScanScene(SceneManager.GetActiveScene());
        }

        public void ScanScene(Scene scene)
        {
            // fill list
            _tmpRootGameObjects.Clear();
            scene.GetRootGameObjects(_tmpRootGameObjects); // May alloc some memory to increase the list size.
            foreach (var go in _tmpRootGameObjects)
            {
                _tmpLights.Clear();
                go.GetComponentsInChildren<Light>(includeInactive: true, _tmpLights);
                foreach (var light in _tmpLights)
                {
                    if(!_lights.Contains(light))
                    {
                        _lights.Add(light);
                        OnNewLightFound?.Invoke(light);
                    }
                }
            }
            _tmpLights.Clear();
            _tmpRootGameObjects.Clear();

            Defrag();
        }

        public void Defrag()
        {
            for (int i = _lights.Count-1; i >= 0; i--)
            {
                if (_lights[i] == null || _lights[i].gameObject == null)
                {
                    _lights.RemoveAt(i);
                }
            }
        }
    }
}
