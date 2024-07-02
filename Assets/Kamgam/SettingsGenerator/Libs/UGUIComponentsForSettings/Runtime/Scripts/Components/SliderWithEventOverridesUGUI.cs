using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class SliderWithEventOverridesUGUI : Slider
    {
        /// <summary>
        /// Returns whether or not the default action should be executed after the event callback.
        /// </summary>
        public System.Func<AxisEventData, bool> OnMoveOverride;

        public override void OnMove(AxisEventData eventData)
        {
            if (OnMoveOverride == null || OnMoveOverride.Invoke(eventData))
                base.OnMove(eventData);
        }
    }
}
