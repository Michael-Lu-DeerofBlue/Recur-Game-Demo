using UnityEngine;
using UnityEngine.UI;

namespace PixelCrushers
{

    /// <summary>
    /// Manages a loading screen progress bar.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class LoadingScreenProgressBar : MonoBehaviour
    {

        [Tooltip("Progress bar slider. Value should should be 0-1.")]
        public Slider slider;

        private void Update()
        {
            if (slider == null) return;
            slider.value = (SaveSystem.currentAsyncOperation != null) ? SaveSystem.currentAsyncOperation.progress : 1;
        }
    }
}
