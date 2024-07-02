using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    public static class RenderPipelineDetector
    {
        public enum RenderPiplelineType
        {
            URP = 0, HDRP = 1, BuiltIn = 2
        }

        public static RenderPiplelineType GetCurrentRenderPiplelineType()
        {
            var currentRP = GraphicsSettings.currentRenderPipeline;

            // currentRP will be null if built-in renderer is used.
            if (currentRP != null)
            {
                if (currentRP.GetType().Name.Contains("Universal"))
                {
                    return RenderPiplelineType.URP;
                }
                else
                {
                    return RenderPiplelineType.HDRP;
                }
            }

            return RenderPiplelineType.BuiltIn;
        }

        public static bool IsURP()
        {
            return GetCurrentRenderPiplelineType() == RenderPiplelineType.URP;
        }

        public static bool IsHDRP()
        {
            return GetCurrentRenderPiplelineType() == RenderPiplelineType.HDRP;
        }

        public static bool IsBuiltIn()
        {
            return GetCurrentRenderPiplelineType() == RenderPiplelineType.BuiltIn;
        }
    }
}