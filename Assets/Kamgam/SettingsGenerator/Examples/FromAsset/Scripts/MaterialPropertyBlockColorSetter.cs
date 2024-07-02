using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    /// <summary>
    /// Caches and then applies changes to a Material by using MaterialPropertyBlocks.
    /// <br />
    /// Use it in situations where you want to draw multiple objects with
    /// the same material, but slightly different properties.
    /// <br />
    /// See: https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html
    /// <br />
    /// <br />
    /// I works by scheduling changes. You will have to call Apply() to
    /// apply these changes to the material. This is a side-effect of how the 
    /// MaterialProperties work (all changes are pushed at once).
    /// </summary>
    public class MaterialPropertyBlockColorSetter : MonoBehaviour
    {
        public Renderer Renderer;
        public int MaterialIndex = 0;

        [System.NonSerialized]
        protected Dictionary<string, Color> _scheduledColors;

        [System.NonSerialized]
        protected MaterialPropertyBlock _propertyBlock;

        public void Init()
        {
            if (Renderer == null)
                Renderer = this.GetComponentInChildren<Renderer>();
        }

#if UNITY_EDITOR
        public void Reset()
        {
            Init();
        }
#endif

        public Material GetSharedMaterial()
        {
            return Renderer.sharedMaterials[MaterialIndex];
        }

        /// <summary>
        /// Are there any scheduled changes?
        /// </summary>
        /// <returns></returns>
        public bool HasScheduledChanges()
        {
            return (_scheduledColors != null && _scheduledColors.Count > 0);
        }

        public bool HasScheduledProperty(string propertyName)
        {
            if (_scheduledColors != null && _scheduledColors.ContainsKey(propertyName))
                return true;

            return false;
        }

        protected void schedule<T>(ref Dictionary<string, T> dict, string propertyName, T value)
        {
            if (dict == null)
                dict = new Dictionary<string, T>();

            addOrUpdateScheduled(dict, propertyName, value);
        }

        protected void addOrUpdateScheduled<T>(Dictionary<string, T> source, string propertyName, T value)
        {
            if (source.ContainsKey(propertyName))
                source[propertyName] = value;
            else
                source.Add(propertyName, value);
        }

        protected T getScheduled<T>(Dictionary<string, T> dict, string propertyName, T defaultValue)
        {
            if (dict == null)
                return defaultValue;

            if (dict.ContainsKey(propertyName))
                return dict[propertyName];

            return defaultValue;
        }
        protected Color getProperty(string propertyName, Color defaultValue = default)
        {
            if (_propertyBlock == null)
                return defaultValue;

            if (!hasColorProperty(propertyName))
                return defaultValue;

            return _propertyBlock.GetColor(propertyName);
        }

        // In Unity version below 2021 there is no Has* API for Properties.
        // Thus we have to keep track of them ourselves.
#if !UNITY_2021_1_OR_NEWER
        protected List<string> _touchedProperties = new List<string>();
#endif

        protected bool hasColorProperty(string propertyName)
        {
#if UNITY_2021_1_OR_NEWER
            return _propertyBlock.HasColor(propertyName);
#else
            return _touchedProperties.Contains(propertyName);
#endif
        }

        protected T get<T>(Dictionary<string, T> dict, string propertyName, System.Func<string, T, T> propertyGetter, T defaultValue = default)
        {
            if (dict != null && dict.ContainsKey(propertyName))
                return getScheduled(dict, propertyName, defaultValue);

            return propertyGetter.Invoke(propertyName, defaultValue);
        }

        /// <summary>
        /// Schedules a color change. Call Apply() to apply it.<br/>
        /// Notice that only the changes within one Schedule(..) -> Apply() cycle are applied, all other properties are reset.
        /// </summary>
        /// <param name="propertyName">An often used name is "_BaseColor" or "_Color".</param>
        /// <param name="color"></param>
        public void ScheduleColor(string propertyName, Color color)
        {
            schedule(ref _scheduledColors, propertyName, color);
        }

        /// <summary>
        /// Returns the currently scheduled color. If none is set then default(Color) is returned.
        /// </summary>
        /// <param name="colorPropertyName">An often used name is "_BaseColor".</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Color GetScheduledColor(string propertyName, Color defaultValue = default)
        {
            return getScheduled(_scheduledColors, propertyName, defaultValue);
        }

        /// <summary>
        /// Returns the currently set color of the PropertyBlock. If none is set then default(Color) is returned.
        /// </summary>
        /// <param name="propertyName">An often used name is "_BaseColor".</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Color GetPropertyColor(string propertyName, Color defaultValue = default)
        {
            return getProperty(propertyName, defaultValue);
        }

        /// <summary>
        /// Returns the scheduled color and if no color is scheduled then return the value of the property block.<br />
        /// If none is set then default(Color) is returned.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Color GetColor(string propertyName, Color defaultValue = default)
        {
            return get(_scheduledColors, propertyName, GetPropertyColor, defaultValue);
        }

        protected void applyList<T>(Dictionary<string, T> dict, System.Action<string, T> setter)
        {
            if (dict != null)
            {
                foreach (var kv in dict)
                {
                    string propertyName = kv.Key;
                    setter.Invoke(propertyName, kv.Value);

#if !UNITY_2021_1_OR_NEWER
                    // Remember for Unity <= 2020
                    if (!_touchedProperties.Contains(propertyName))
                        _touchedProperties.Add(propertyName);
#endif
                }
            }
        }

        /// <summary>
        /// Applies all scheduled changes but does NOT clear the scheduled change list.
        /// </summary>
        public void Apply()
        {
            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            if (Renderer == null)
                throw new System.Exception("Renderer is null.");

            // get block
            Renderer.GetPropertyBlock(_propertyBlock, MaterialIndex);

            applyList(_scheduledColors, _propertyBlock.SetColor);

            // set block
            Renderer.SetPropertyBlock(_propertyBlock, MaterialIndex);
        }

        /// <summary>
        /// Clears the cache which holds the scheduled changes.
        /// </summary>
        public void ClearScheduled()
        {
            if (_scheduledColors != null) _scheduledColors.Clear();
        }

        /// <summary>
        /// Resets the properties.<br />
        /// Does not affect the scheduled changes.
        /// It simply creates a new empty property block and applies that.
        /// All scheduled changes will remain scheduled.
        /// </summary>
        public void ResetProperties()
        {
            if (Renderer == null)
                throw new System.Exception("Renderer is null.");

            _propertyBlock = new MaterialPropertyBlock();
            Renderer.SetPropertyBlock(_propertyBlock, MaterialIndex);
        }

        public void SetMainColor(Color color)
        {
            string colorPropertyName = GetMainColorPropertyName();
            ScheduleColor(colorPropertyName, color);
            Apply();
        }

        private readonly string[] _colorPropertyNames = {
            "_BaseColor",
            "_MainColor",
            "_Color"
        };

        public string GetMainColorPropertyName()
        {
            if (Renderer == null)
                return null;

            var mat = Renderer.sharedMaterial;
            foreach (var propName in _colorPropertyNames)
            {
                if (mat.HasProperty(propName))
                {
                    return propName;
                }
            }

            return null;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MaterialPropertyBlockColorSetter))]
    public class MaterialPropertyBlockSetterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var setter = target as MaterialPropertyBlockColorSetter;

            if (GUILayout.Button("Reset Properties"))
            {
                setter.ResetProperties();
                RepaintViews();
            }

            if (GUILayout.Button("Test: Apply random color to _Color"))
            {
                setter.ScheduleColor(setter.GetMainColorPropertyName(), new Color(Random.value, Random.value, Random.value, Random.value));
                setter.Apply();
                RepaintViews();
            }
        }

        public static void RepaintViews()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            UnityEditor.EditorWindow gameview = UnityEditor.EditorWindow.GetWindow(type);
            gameview.Repaint();

            UnityEditor.EditorWindow view = UnityEditor.EditorWindow.GetWindow<UnityEditor.SceneView>();
            view.Repaint();
        }
    }
#endif
}
