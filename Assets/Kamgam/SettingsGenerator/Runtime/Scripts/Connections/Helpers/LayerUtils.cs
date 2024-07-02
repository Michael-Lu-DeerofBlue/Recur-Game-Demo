using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class LayerUtils
    {
        /// <summary>
        /// Find the index of the first layer included in the mask.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="defaultIndex"></param>
        /// <returns></returns>
        public static int GetIndexOfFirstLayerInMask(LayerMask mask, int defaultIndex = -1)
        {
            // Find the first layer included in the mask and use it.
            for (int i = 0; i < 32; i++)
            {
                int layer = 1 << i;
                if ((mask & layer) > 0)
                {
                    return i;
                }
            }

            return defaultIndex;
        }
    }
}
