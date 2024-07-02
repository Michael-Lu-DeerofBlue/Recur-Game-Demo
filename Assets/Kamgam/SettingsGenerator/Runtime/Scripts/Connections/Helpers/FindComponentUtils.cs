using System.Collections.Generic;
using System.Linq;

namespace Kamgam.SettingsGenerator
{
    public static class FindComponentUtils
    {
        /// <summary>
        /// Returns a Component of type T in all loaded scenes.
        /// Be aware that this may also return an object which is scheduled for destruction at the end of the current frame.
        /// Be double aware that this will NOT return any object marked as "DontDestroyOnLoad" since those are moved to a
        /// unique scene which is not accessible during runtime.
        /// See: https://gamedev.stackexchange.com/questions/140014/how-can-i-get-all-dontdestroyonload-gameobjects
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scenePredicate">Use this to exclude scenes from being searched.</param>
        /// <returns></returns>
        public static T FindComponentInAllLoadedScenes<T>(bool includeInactive, System.Predicate<UnityEngine.SceneManagement.Scene> scenePredicate = null)
        {
            var components = FindComponentsInAllLoadedScenes<T>(includeInactive, scenePredicate);
            if (components.Count > 0)
            {
                return components[0];
            }

            return default(T);
        }

        /// <summary>
        /// Returns all Components of type T in all loaded scenes.
        /// Be aware that this may also return objects which are scheduled for destruction at the end of the current frame.
        /// Be double aware that this will NOT return any objects marked as "DontDestroyOnLoad" since those are moved to a
        /// unique scene which is not accessible during runtime.
        /// See: https://gamedev.stackexchange.com/questions/140014/how-can-i-get-all-dontdestroyonload-gameobjects
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scenePredicate">Use this to exclude scenes from being searched.</param>
        /// <returns></returns>
        public static List<T> FindComponentsInAllLoadedScenes<T>(bool includeInactive, System.Predicate<UnityEngine.SceneManagement.Scene> scenePredicate = null)
        {
            // TODO: For Unity2020+ we could use Object.FindObjectsOfType with includeInactive = true instead.
            UnityEngine.SceneManagement.Scene[] scenes = new UnityEngine.SceneManagement.Scene[UnityEngine.SceneManagement.SceneManager.sceneCount];
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scenePredicate == null || scenePredicate(scene))
                {
                    scenes[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                }
            }

            return FindComponentsInScenes<T>(includeInactive, scenes);
        }

        public static List<T> FindComponentsInScenes<T>(bool includeInactive, params UnityEngine.SceneManagement.Scene[] scenes)
        {
            try
            {
                return scenes
                    .Where(s => s.IsValid())
                    .SelectMany(s => s.GetRootGameObjects())
                    .Where(g => includeInactive || g.activeInHierarchy)
                    .SelectMany(g => g.GetComponentsInChildren<T>(includeInactive))
                    .ToList();
            }
            catch (System.Exception)
            {
                return new List<T>();
            }
        }
    }
}