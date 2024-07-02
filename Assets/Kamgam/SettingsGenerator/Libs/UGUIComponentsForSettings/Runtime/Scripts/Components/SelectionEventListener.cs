using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    [RequireComponent(typeof(Selectable))]
    public class SelectionEventListener : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public delegate void OnSelectionChangedDelegate(bool isSelected);
        public UnityEvent<bool> OnSelectionChangedEvent;
        public OnSelectionChangedDelegate OnSelectionChanged;

        protected Selectable selectable;
        public Selectable Selectable
        {
            get
            {
                if (selectable == null)
                {
                    selectable = this.GetComponent<Selectable>();
                }
                return selectable;
            }
        }

        public bool IsSelected
        {
            get
            {
                if (EventSystem.current == null)
                    return false;

                if (EventSystem.current.currentSelectedGameObject == null)
                    return false;

                return EventSystem.current.currentSelectedGameObject == Selectable;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectionChanged?.Invoke(true);
            OnSelectionChangedEvent?.Invoke(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnSelectionChanged?.Invoke(false);
            OnSelectionChangedEvent?.Invoke(false);
        }
    }
}
