using UnityEngine;
using UnityEngine.EventSystems;

namespace Kamgam.UGUIComponentsForSettings
{
    public static class SelectionUtils
    {
        public static void SetSelected(GameObject go, bool triggerOnReselect = true)
        {
            if (EventSystem.current != null && go != null && !EventSystem.current.alreadySelecting)
            {
                bool wasSelected = EventSystem.current.currentSelectedGameObject == go;
                EventSystem.current.SetSelectedGameObject(go);
                // Ensure that even for reselection the select handler is being fired.
                if (wasSelected && triggerOnReselect)
                {
                    ExecuteEvents.ExecuteHierarchy(go, new BaseEventData(EventSystem.current), ExecuteEvents.selectHandler);
                }
            }
        }
    }
}
