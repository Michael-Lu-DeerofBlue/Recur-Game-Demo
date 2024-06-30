using UnityEngine;
#if USE_CINEMACHINE
#if UNITY_6000_0_OR_NEWER
using Unity.Cinemachine;
using CinemachineCam = Unity.Cinemachine.CinemachineCamera;
#else
using Cinemachine;
using CinemachineCam = Cinemachine.CinemachineVirtualCamera;
#endif
#endif

namespace PixelCrushers.DialogueSystem
{

#if USE_CINEMACHINE

    [AddComponentMenu("")] // Use wrapper.
    public class CinemachineCameraPriorityOnDialogueEvent : ActOnDialogueEvent
    {

        [Tooltip("The Cinemachine virtual camera whose priority to control.")]
        public CinemachineCam virtualCamera;

        [Tooltip("Set the virtual camera to this priority when the start event occurs.")]
        public int onStart = 99;

        [Tooltip("Set the virtual camera to this priority when the end event occurs.")]
        public int onEnd = 0;

        public override void TryStartActions(Transform actor)
        {
            if (virtualCamera == null) return;
            virtualCamera.Priority = onStart;
        }

        public override void TryEndActions(Transform actor)
        {
            if (virtualCamera == null) return;
            virtualCamera.Priority = onEnd;
        }
    }

#else

    [AddComponentMenu("")] // Use wrapper.
    public class CinemachineCameraPriorityOnDialogueEvent : ActOnDialogueEvent
    {
        public override void TryStartActions(Transform actor) { }
        public override void TryEndActions(Transform actor) { }
    }

#endif

}
