using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// If this is a child of a ScrollView then it brings
    /// the child into view if it is selected (OnSelect).
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class ScrollIntoViewOnSelectUGUI : MonoBehaviour, ISelectHandler
    {
        [Tooltip("If turned off then it will do nothing.")]
        public bool Enabled = true;

        [Tooltip("Additional margins in clockwise order: TOP, RIGHT, BOTTOM, LEFT")]
        public Vector4 MarginTRBL;

        public void OnSelect(BaseEventData eventData)
        {
            if (!Enabled)
                return;

            var scrollRect = transform.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                BringChildIntoView(scrollRect, transform as RectTransform, MarginTRBL);
            }
        }

        /// <summary>
        /// Moves the content to bring the Rect of "child" into viewport (moves only if needed).
        /// </summary>
        /// <param name="instance">The ScrollRect</param>
        /// <param name="child">It does not matter how deep the child is nested.</param>
        /// <param name="margin">Margins in clockwise order: TOP, RIGHT, BOTTOM, LEFT</param>
        /// <returns></returns>
        public static void BringChildIntoView(UnityEngine.UI.ScrollRect instance, RectTransform child, Vector4 margin)
        {
            instance.content.ForceUpdateRectTransforms();
            instance.viewport.ForceUpdateRectTransforms();

            Rect childRectInViewportLocalCoords = TransformRectFrom(instance.viewport, child);
            childRectInViewportLocalCoords.xMin -= margin[3];
            childRectInViewportLocalCoords.xMax += margin[1];
            childRectInViewportLocalCoords.yMin -= margin[2];
            childRectInViewportLocalCoords.yMax += margin[0];
            Rect viewportRectInViewportLocalCoords = instance.viewport.rect;
            var newContentPosition = instance.content.localPosition;

            // update content postition based on viewport and child (clamp to viewport)
            bool moveNeeded = false;
            float deltaXMin = viewportRectInViewportLocalCoords.xMin - childRectInViewportLocalCoords.xMin;
            if (deltaXMin > 0.001f) // clamp to <= 0
            {
                newContentPosition.x += deltaXMin;
                moveNeeded = true;
            }
            float deltaXMax = viewportRectInViewportLocalCoords.xMax - childRectInViewportLocalCoords.xMax;
            if (deltaXMax < -0.001f) // clamp to >= 0
            {
                newContentPosition.x += deltaXMax;
                moveNeeded = true;
            }
            float deltaYMin = viewportRectInViewportLocalCoords.yMin - childRectInViewportLocalCoords.yMin;
            if (deltaYMin > 0.001f) // clamp to <= 0
            {
                newContentPosition.y += deltaYMin;
                moveNeeded = true;
            }
            float deltaYMax = viewportRectInViewportLocalCoords.yMax - childRectInViewportLocalCoords.yMax;
            if (deltaYMax < -0.001f) // clamp to >= 0
            {
                newContentPosition.y += deltaYMax;
                moveNeeded = true;
            }

            // apply final position
            if (moveNeeded)
            {
                instance.content.localPosition = newContentPosition;
                instance.content.ForceUpdateRectTransforms();
            }
        }

        /// <summary>
        /// Converts a Rect from one RectTransfrom to this RectTransfrom (as if the "from" is a child of this).
        /// Hint: use the Canvas Transform as "to" to get the reference pixel positions.
        /// Similar to: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/UI/ScriptBindings/RectTransformUtility.cs
        /// </summary>
        /// <param name="from">The rect transform which should be transformed as if it was a child(!) of this transform.</param>
        /// <returns></returns>
        public static Rect TransformRectFrom(Transform to, Transform from)
        {
            RectTransform fromRectTrans = from.GetComponent<RectTransform>();
            RectTransform toRectTrans = to.GetComponent<RectTransform>();

            if (fromRectTrans != null && toRectTrans != null)
            {
                Vector3[] fromWorldCorners = new Vector3[4];
                Vector3[] toLocalCorners = new Vector3[4];
                Matrix4x4 toLocal = to.worldToLocalMatrix;
                fromRectTrans.GetWorldCorners(fromWorldCorners);
                for (int i = 0; i < 4; i++)
                {
                    toLocalCorners[i] = toLocal.MultiplyPoint3x4(fromWorldCorners[i]);
                }

                return new Rect(toLocalCorners[0].x, toLocalCorners[0].y, toLocalCorners[2].x - toLocalCorners[1].x, toLocalCorners[1].y - toLocalCorners[0].y);
            }

            return default(Rect);
        }
    }
}