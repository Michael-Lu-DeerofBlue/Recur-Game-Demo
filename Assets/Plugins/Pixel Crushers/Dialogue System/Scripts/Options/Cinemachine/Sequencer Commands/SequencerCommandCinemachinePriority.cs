#if USE_CINEMACHINE
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using Unity.Cinemachine;
using CinemachineCam = Unity.Cinemachine.CinemachineCamera;
#else
using Cinemachine;
using CinemachineCam = Cinemachine.CinemachineVirtualCamera;
#endif

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer commannd CinemachinePriority(vcam, [priority], [cut])
    /// 
    /// Sets the priority of a Cinemachine virtual camera.
    /// 
    /// - vcam: The name of a GameObject containing a CinemachineVirtualCamera.
    ///    Alternate: 'all' or 'except:GameObject': All affects all vcams; except affects all except one.
    /// - priority: (Optional) New priority level. Default: 999.
    /// - cut: (Optional) Cuts to the vcam instead of allowing Cinemachine to ease there.
    /// </summary>
    public class SequencerCommandCinemachinePriority : SequencerCommand
    {
        private static bool hasRecordedBlendMode = false;

        public System.Collections.IEnumerator Start()
        {
            bool all = false;
            string allExcept = string.Empty;
            bool checkExcept = false;
            CinemachineCam vcam = null;

            var vcamName = GetParameter(0);
            if (vcamName == "all")
            {
                all = true;
            }
            else if (vcamName.StartsWith("except:"))
            {
                all = true;
                checkExcept = true;
                allExcept = vcamName.Substring("except:".Length);
            }
            else
            {
                var subject = GetSubject(0);
                vcam = (subject != null) ? subject.GetComponent<CinemachineCam>() : null;
            }
            var priority = GetParameterAsInt(1, 999);
            var cut = string.Equals(GetParameter(2), "cut", System.StringComparison.OrdinalIgnoreCase);
            if (!all && vcam == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: CinemachinePriority(" + GetParameters() +
                    "): Can't find virtual camera '" + GetParameter(0) + ".");
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: CinemachinePriority(" + vcamName + ", " + priority + ", cut=" + cut + ")");

                // Handle cut:
                var shouldIRestoreBlendMode = false;
                var cinemachineBrain = cut ? GameObjectUtility.FindFirstObjectByType<CinemachineBrain>() : null;
#if UNITY_6000_0_OR_NEWER
                var previousBlendStyle = CinemachineBlendDefinition.Styles.EaseInOut;
#else
                var previousBlendStyle = CinemachineBlendDefinition.Style.EaseInOut;
#endif
                var previousBlendTime = 0f;
                if (cut && cinemachineBrain != null)
                {
                    shouldIRestoreBlendMode = !hasRecordedBlendMode;
                    hasRecordedBlendMode = true;
#if UNITY_6000_0_OR_NEWER
                    previousBlendStyle = cinemachineBrain.DefaultBlend.Style;
                    previousBlendTime = cinemachineBrain.DefaultBlend.Time;
                    cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
                    cinemachineBrain.DefaultBlend.Time = 0;
#else
                    previousBlendStyle = cinemachineBrain.m_DefaultBlend.m_Style;
                    previousBlendTime = cinemachineBrain.m_DefaultBlend.m_Time;
                    cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    cinemachineBrain.m_DefaultBlend.m_Time = 0;
#endif
                    cinemachineBrain.enabled = false;
                }

                if (all)
                {
                    var allVcams = GameObjectUtility.FindObjectsByType<CinemachineCam>();
                    foreach (CinemachineCam avcam in allVcams)
                    {
                        if (checkExcept && string.Equals(avcam.name, allExcept)) continue;
                        avcam.Priority = priority;
                        if (cut)
                        {
                            avcam.enabled = false;
                            avcam.enabled = true;
                        }
                    }
                }
                else
                {
                    vcam.Priority = priority;
                    if (cut)
                    {
                        vcam.enabled = false;
                        vcam.enabled = true;
                    }
                }

                // Clean up cut:
                if (cut && cinemachineBrain != null)
                {
                    cinemachineBrain.enabled = true;
                    if (shouldIRestoreBlendMode)
                    {
                        yield return null;
#if UNITY_6000_0_OR_NEWER
                        cinemachineBrain.DefaultBlend.Style = previousBlendStyle;
                        cinemachineBrain.DefaultBlend.Time = previousBlendTime;
#else
                        cinemachineBrain.m_DefaultBlend.m_Style = previousBlendStyle;
                        cinemachineBrain.m_DefaultBlend.m_Time = previousBlendTime;
#endif
                        hasRecordedBlendMode = false;
                    }
                }
            }
            Stop();
        }

    }

}
#endif
