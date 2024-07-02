// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using System.Collections.Generic;
using UnityEngine;

#if KAMGAM_POST_PRO_BUILTIN
using UnityEngine.Rendering.PostProcessing;
#endif


namespace Kamgam.SettingsGenerator
{
    public partial class AntiAliasingConnection : ConnectionWithOptions<string>
    {
        protected List<int> _values;
        protected List<string> _labels;

        protected bool _editorPostProWarningShown = false;

        protected List<int> getOptionValues()
        {
            if (_values == null)
            {
                _values = new List<int>();
                _values.Add(0);
                _values.Add(2);
                _values.Add(4);
                _values.Add(8);
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>();
                _labels.Add("Disabled");

                var optionValues = getOptionValues();
                for (int i = 1; i < optionValues.Count; i++)
                {
                    string name = optionValues[i].ToString() + "x MSAA";
                    _labels.Add(name);
                }
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getOptionValues();
            if (optionLabels == null || optionLabels.Count != values.Count)
            {
                Debug.LogError("Invalid new labels. Need to be " + values.Count + ".");
                return;
            }

            _labels = new List<string>(optionLabels);
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
            var optionValues = getOptionValues();
            for (int i = 0; i < optionValues.Count; i++)
            {
                if (optionValues[i] == QualitySettings.antiAliasing)
                {
                    return i;
                }
            }

            return 0;
        }

        public override void Set(int index)
        {
            var optionValues = getOptionValues();
            index = Mathf.Clamp(index, 0, optionValues.Count - 1);
            var value = optionValues[index];

            QualitySettings.antiAliasing = value;

            warnIfAntiAliasingIsEnabledOnCameraInEditor();

            NotifyListenersIfChanged(index);
        }

        protected void warnIfAntiAliasingIsEnabledOnCameraInEditor()
        {
            // Anti Aliasing can be changed on a per camera (or volume) basis in
            // PostPro. If that is used then warn the user.

            if (_editorPostProWarningShown)
                return;

#if KAMGAM_POST_PRO_BUILTIN
            if (Camera.main != null)
            {
                var postProLayer = Camera.main.gameObject.GetComponent<PostProcessLayer>();
                if (postProLayer != null && postProLayer.antialiasingMode != PostProcessLayer.Antialiasing.None)
                {
                    Debug.LogWarning("AntiAliasingConnection: The anti aliasing setting is " +
                                     "overwritten in the Post-process layer on the camera ('" + Camera.main.name + "')." +
                                     " This will override your global settings. If you want to " +
                                     "change the anti aliasing for the camera then you will have " +
                                     "to do this manually." +
                                     "\n\nNOTICE: You should not enable anti aliasing in " +
                                     "your global settings AND on your camera at the same time. " +
                                     "This will apply anti aliasing twice. First your global version " +
                                     "and then the camera version on top of that. ");

                    _editorPostProWarningShown = true;
                }
            }
#endif
        }
    }
}
#endif