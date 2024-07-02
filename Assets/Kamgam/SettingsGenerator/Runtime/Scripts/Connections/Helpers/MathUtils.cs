using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class MathUtils
    {
        /// <summary>
        /// Maps a single value to one of two ranges (below, above).
        /// The anchor point remains unchanged (think of it as the scale center).
        /// The range below the anchor can differ from the range above the anchor.
        /// </summary>
        /// <param name="inValue"></param>
        /// <param name="inMin"></param>
        /// <param name="inAnchor">The anchor has to be between inMin and inMax.</param>
        /// <param name="inMax"></param>
        /// <param name="outMin"></param>
        /// <param name="outAnchor">The anchor has to be between outMin and outMax.</param>
        /// <param name="outMax"></param>
        /// <param name="clamp"></param>
        /// <returns></returns>
        public static float MapWithAnchor(float inValue, float inMin, float inAnchor, float inMax, float outMin, float outAnchor, float outMax, bool clamp = true)
        {
            if (inMin > inAnchor || inMin >= inMax)
                throw new System.Exception($"inMin ({inMin}) has to be below inAnchor ({inAnchor}) and inMax ({inMax})");

            if (outMin > outAnchor || outMin >= outMax)
                throw new System.Exception($"outMin ({outMin}) has to be below outAnchor ({outAnchor}) and outMax ({outMax})");

            if (Mathf.Approximately(inValue,inAnchor))
                return outAnchor;

            if (Mathf.Approximately(inValue, inMin) || (clamp && inValue < inMin))
                return outMin;

            if (Mathf.Approximately(inValue, inMax) || (clamp && inValue > inMax))
                return outMax;

            float distanceToInAnchor = Mathf.Abs(inAnchor - inValue);
            float maxDistanceToInAnchor;
            float maxDistanceToOutAnchor;
            float scale;
            if (inValue < inAnchor)
            {
                maxDistanceToInAnchor = inAnchor - inMin;
                scale = distanceToInAnchor / maxDistanceToInAnchor;
                maxDistanceToOutAnchor = outMin - outAnchor; // result is negative
            }
            else
            {
                maxDistanceToInAnchor = inMax - inAnchor;
                scale = distanceToInAnchor / maxDistanceToInAnchor;
                maxDistanceToOutAnchor = outMax - outAnchor;
            }
            return outAnchor + maxDistanceToOutAnchor * scale;
        }
    }
}
