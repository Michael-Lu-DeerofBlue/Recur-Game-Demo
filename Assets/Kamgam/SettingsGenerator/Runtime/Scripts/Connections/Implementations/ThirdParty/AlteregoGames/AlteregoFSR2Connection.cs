using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
using AEG.FSR;
#endif

#if AEG_FSR1 && !AEG_FSR2 && !AEG_FSR3
using FSR_BASE = AEG.FSR.FSR1_BASE;
#endif

#if AEG_FSR2 && !AEG_FSR3
using FSR_BASE = AEG.FSR.FSR2_BASE;
#endif

#if AEG_FSR3
using FSR_BASE = AEG.FSR.FSR3_BASE;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// While the name of this class is AlteregoFSR2Connection it also covers FSR3.
    /// The name remained the same for backwards compatibility reasons.
    /// </summary>
    public class AlteregoFSR2Connection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;

        public AlteregoFSR2Connection()
        {
#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
            if (!IsSupported())
            {
                Logger.LogWarning("AlteregoFSR2Connection: Alterego FSR2 is not supported. Please contact Alterego Games for more info and support.");
            }
#else
            Logger.LogWarning("AlteregoFSR2Connection: Alterego FSR2 is not yet set up. Please consult the Alterego Games Manual for more info and support.");
#endif
        }

#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
        public FSR_BASE GetUtils()
        {
            FSR_BASE utils = null;
            
            var cam = Camera.main;
            if (cam != null)
                utils = cam.GetComponent<FSR_BASE>();
            if (utils != null)
                return utils;

            cam = Camera.current;
            if (cam != null)
                utils = cam.GetComponent<FSR_BASE>();

            if (utils == null)
            {
                Logger.LogWarning("AlteregoFSR2Connection: Could not find the FSR component on the current camera. Please make sure you have FSR added (ignoring all FSR settings for now).");
            }

            return utils;
        }

#endif

        public bool IsSupported()
        {
#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
            var utils = GetUtils();
            if (utils == null)
                return false;

            return utils.OnIsSupported();
#else
            return false;
#endif
        }

        public override List<string> GetOptionLabels()
        {
#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
            if (!IsSupported())
            {
                if(_labels == null || _labels.Count == 0)
                    _labels = new List<string>() { "Not Supported" };
                return _labels;
            }

            if (_labels == null)
            {
                _labels = new List<string>();
                var qualityNames = System.Enum.GetNames(typeof(FSR_Quality));
                foreach (var name in qualityNames)
                {
                    _labels.Add(name);
                }
            }
#else
            Logger.LogWarning("AlteregoFSR2Connection: Alterego FSR is not yet set up. Please consult the Alterego Games Manual for more info and support.");
#endif
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != 4)
            {
                Debug.LogError("Invalid new labels. Need to be four.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
            var utils = GetUtils();
            if (utils != null)
            {
                return (int)utils.FSRQuality;
            }
            else
            {
                return 0;
            }
#else
            Logger.LogWarning("AlteregoFSR2Connection: Alterego FSR2 is not yet set up. Please consult the Alterego Games Manual for more info and support.");
            return 0;
#endif
        }

        public override void Set(int index)
        {
#if AEG_FSR1 || AEG_FSR2 || AEG_FSR3
            var utils = GetUtils();
            if (utils != null)
            {
                utils.FSRQuality = (FSR_Quality)index;
            }
#endif
            NotifyListenersIfChanged(index);
        }
    }
}