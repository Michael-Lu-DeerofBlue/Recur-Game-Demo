using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// The selection my sometimes become null. Use this component to fix this.
    /// </summary>
    public class SelectIfNull : MonoBehaviour
    {
        public enum Trigger { Awake, OnEnable, Start, Update, LateUpdate, OnDirectionInput }

        [Tooltip("Defines when the select if null logic should be executed. You can pick one or more times.")]
        public Trigger[] Triggers;

        [Tooltip("Optional: If candidates are set then these are the preferred target for the selection. A candidate will only be selected if it is enable, active and interactable.")]
        public Selectable[] Candidates;

        [Tooltip("Search for any interactable selectable if none of the Candidates is valid?")]
        public bool SearchForSelectables = true;

        public void Awake()
        {
            if (containsTrigger(Trigger.Awake))
                selectIfNull();
        }

        public void Start()
        {
            if (containsTrigger(Trigger.Start))
                selectIfNull();
        }

        public void OnEnable()
        {
            if (containsTrigger(Trigger.OnEnable))
                selectIfNull();
        }

        public void Update()
        {
            if (containsTrigger(Trigger.Update))
                selectIfNull();

            if (containsTrigger(Trigger.OnDirectionInput) && InputUtils.AnyDirection())
            {
                selectIfNull();
            }
        }

        public void LateUpdate()
        {
            if (containsTrigger(Trigger.LateUpdate))
                selectIfNull();
        }

        bool containsTrigger(Trigger trigger)
        {
            if (Triggers == null || Triggers.Length == 0)
                return false;

            foreach (var t in Triggers)
            {
                if (t == trigger)
                    return true;
            }
            return false;
        }

        void selectIfNull()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            {
                // Find first valid selectable candidate.
                Selectable obj = null;
                foreach (var c in Candidates)
                {
                    if(c.enabled && c.interactable && c.gameObject.activeInHierarchy)
                    {
                        obj = c;
                        break;
                    }
                }

                // Search if there is no valid candidate.
                if (SearchForSelectables && obj == null)
                {
                    var selectables = Selectable.allSelectablesArray;
                    foreach (var selectable in selectables)
                    {
                        if (selectable.isActiveAndEnabled && selectable.interactable && selectable.gameObject.activeInHierarchy)
                        {
                            obj = selectable;
                            break;
                        }
                    }
                }

                if (obj != null)
                {
                    EventSystem.current.SetSelectedGameObject(obj.gameObject);
                }
            }
        }

#if UNITY_EDITOR
        public void Reset()
        {
            if (Triggers == null || Triggers.Length == 0)
            {
                Triggers = new Trigger[] { Trigger.OnDirectionInput };
            }

            if (Candidates == null || Candidates.Length == 0)
            {
                var comp = GetComponentInChildren<Selectable>(includeInactive: false);
                if (comp != null)
                {
                    Candidates = new Selectable[] { comp };
                }
                else
                {
                    comp = GetComponentInChildren<Selectable>(includeInactive: true);
                    if (comp != null)
                    {
                        Candidates = new Selectable[] { comp };
                    }
                }
            }
        }
#endif

    }
}
