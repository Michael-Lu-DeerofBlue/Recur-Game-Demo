using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// If the currently selected Selectable is set to "not interactable" then usually the
    /// current selection gets reset to NULL.
    /// Add this component if you want the Selectable to still be selected even if it was
    /// set to "not interactable". This way you can avoid the current selection becoming null.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class SelectionLingerer : MonoBehaviour
    {
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

        protected bool _selectableIsInteractable;

        public void OnEnable()
        {
            if (Selectable == null)
                return;

            _selectableIsInteractable = Selectable.interactable;
        }

        public void Update()
        {
            if (Selectable == null)
                return;

            // Did interactable state change?
            if (_selectableIsInteractable != Selectable.interactable)
            {
                _selectableIsInteractable = Selectable.interactable;
                if (!_selectableIsInteractable && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                {
                    SelectionUtils.SetSelected(Selectable.gameObject);
                }
            }
        }
    }
}
