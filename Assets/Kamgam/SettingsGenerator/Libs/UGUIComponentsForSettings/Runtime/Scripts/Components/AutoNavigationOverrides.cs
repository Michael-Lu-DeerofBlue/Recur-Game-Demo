using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// TLDR: Use this instead of the "explicit" navigation mode.<br />
    /// <br />
    /// Usually auto navigatoin does a fine job, but sometimes it is better 
    /// to set the selection by hand. The simple approach would be to use
    /// "explicit" mode instead. Sadly in explicit mode the selection will just
    /// do nothing if the target is disabled. It would be more desirable to fall
    /// back on dynamic selection if the explicit selection does not find a valid
    /// target. This is what the overrides are for. Think of the as explicit values
    /// which fall back on auto select if the specified target can not be selected.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class AutoNavigationOverrides : MonoBehaviour, ISelectHandler, IUpdateSelectedHandler
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

        public bool DisableOnAwakeIfNotNeeded = true;

        [Tooltip("Defines which element to navigate to.<br />If left empty then the default navigatoin will be used.")]
        public Selectable SelectOnUpOverride;

        [Tooltip("Defines which element to navigate to.<br />If left empty then the default navigatoin will be used.")]
        public Selectable SelectOnDownOverride;

        [Tooltip("Defines which element to navigate to.<br />If left empty then the default navigatoin will be used.")]
        public Selectable SelectOnLeftOverride;

        [Tooltip("Defines which element to navigate to.<br />If left empty then the default navigatoin will be used.")]
        public Selectable SelectOnRightOverride;

        public bool BlockUp = false;
        public bool BlockDown = false;
        public bool BlockLeft = false;
        public bool BlockRight = false;

        public bool IsBlockingAnyDirection => BlockUp || BlockDown || BlockLeft || BlockRight;

        public void Awake()
        {
            // Disabled self if mode is explicit.
            if (Selectable != null && Selectable.navigation.mode == Navigation.Mode.Explicit)
            {
                this.enabled = false;
            }

            // Disabled self if no overide are specified
            if (DisableOnAwakeIfNotNeeded && !HasOverrides() && !IsBlockingAnyDirection)
            {
                this.enabled = false;
            }
        }

        public bool HasOverrides()
        {
            return SelectOnUpOverride != null || SelectOnDownOverride != null || SelectOnLeftOverride != null || SelectOnRightOverride != null;
        }

        public bool HasActiveOverrides()
        {
            return (SelectOnUpOverride != null && SelectOnUpOverride.isActiveAndEnabled)
                || (SelectOnDownOverride != null && SelectOnDownOverride.isActiveAndEnabled)
                || (SelectOnLeftOverride != null && SelectOnLeftOverride.isActiveAndEnabled)
                || (SelectOnRightOverride != null && SelectOnRightOverride.isActiveAndEnabled);
        }

        // This is executed every frame while the Selectable is selected.
        public void OnUpdateSelected(BaseEventData eventData)
        {
            ApplyOverrides();
        }

        public void ApplyOverrides()
        {
            if (Selectable == null)
                return;

            Navigation navigation;

            // Revert back to automatic and abort if no valid overrides are found.
            if (!HasActiveOverrides() && !IsBlockingAnyDirection)
            {
                navigation = Selectable.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                Selectable.navigation = navigation;
                return;
            }

            // Fetch Defaults
            navigation = Selectable.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            Selectable.navigation = navigation;
            var selectOnUp = Selectable.FindSelectableOnUp();
            var selectOnDown = Selectable.FindSelectableOnDown();
            var selectOnLeft = Selectable.FindSelectableOnLeft();
            var selectOnRight = Selectable.FindSelectableOnRight();

            // Change to explicit
            navigation = Selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;

            // Copy in defaults
            navigation.selectOnUp = selectOnUp;
            navigation.selectOnDown = selectOnDown;
            navigation.selectOnLeft = selectOnLeft;
            navigation.selectOnRight = selectOnRight;

            // Apply overrides
            if (HasOverrides())
            {
                if (SelectOnUpOverride != null && SelectOnUpOverride.interactable)
                {
                    navigation.selectOnUp = SelectOnUpOverride;
                }
                if (SelectOnDownOverride != null && SelectOnDownOverride.interactable)
                {
                    navigation.selectOnDown = SelectOnDownOverride;
                }
                if (SelectOnLeftOverride != null && SelectOnLeftOverride.interactable)
                {
                    navigation.selectOnLeft = SelectOnLeftOverride;
                }
                if (SelectOnRightOverride != null && SelectOnRightOverride.interactable)
                {
                    navigation.selectOnRight = SelectOnRightOverride;
                }
            }

            if (BlockUp) navigation.selectOnUp = null;
            if (BlockDown) navigation.selectOnDown = null;
            if (BlockLeft) navigation.selectOnLeft = null;
            if (BlockRight) navigation.selectOnRight = null;

            // Apply
            Selectable.navigation = navigation;
        }

        public void OnSelect(BaseEventData eventData)
        {
            ApplyOverrides();
        }
    }
}
