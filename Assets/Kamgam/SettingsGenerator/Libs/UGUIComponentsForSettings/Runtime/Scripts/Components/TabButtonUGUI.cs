using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    public class TabButtonUGUI : MonoBehaviour
    {
        public int GroupID = 0;

        public GameObject Normal;
        public GameObject Active;
        public GameObject Content;

        public bool IsActive => Active.activeSelf;

        public TextMeshProUGUI NormalTextTf;
        public TextMeshProUGUI ActiveTextTf;

        public string Text
        {
            get => NormalTextTf.text;
            set
            {
                if (value == Text)
                    return;

                NormalTextTf.text = value;
                ActiveTextTf.text = value;
            }
        }

        public void SetActive(bool active)
        {
            SetActive(active, false);
        }

        public void SetActive(bool active, bool includeInactiveSiblings)
        {
            setActiveInternal(active);
            UpdateSiblings(includeInactiveSiblings);
        }

        protected void setActiveInternal(bool active)
        {
            Normal.gameObject.SetActive(!active);
            Active.gameObject.SetActive(active);

            Content.gameObject.SetActive(active);
        }

        public void UpdateSiblings(bool includeInactive = false)
        {
            var siblings = FindSiblings(includeInactive);
            foreach (var btn in siblings)
            {
                if (btn != this)
                {
                    btn.setActiveInternal(false);
                }
            }
        }

        public List<TabButtonUGUI> FindSiblings(bool includeInactive = false)
        {
            var siblings = new List<TabButtonUGUI>();

            var startTransform = transform.parent;
            if (startTransform == null)
            {
                var roots = transform.gameObject.scene.GetRootGameObjects();
                for (int i = 0; i < roots.Length; i++)
                {
                    var btn = roots[i].GetComponent<TabButtonUGUI>();
                    if (btn != null && btn.GroupID == GroupID && (btn.gameObject.activeSelf || includeInactive))
                    {
                        siblings.Add(btn);
                    }
                }
            }
            else
            {
                for (int i = 0; i < startTransform.childCount; i++)
                {
                    var btn = startTransform.GetChild(i).GetComponent<TabButtonUGUI>();
                    if (btn != null && btn.GroupID == GroupID && (btn.gameObject.activeSelf || includeInactive))
                    {
                        siblings.Add(btn);
                    }
                }
            }

            return siblings;
        }
    }
}
