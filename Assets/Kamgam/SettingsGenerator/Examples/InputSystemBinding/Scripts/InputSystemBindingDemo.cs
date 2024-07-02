using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    public class InputSystemBindingDemo : MonoBehaviour
    {
        public SettingsProvider Provider;

        public void Awake()
        {
            // You may think about adding it to Awake(). Don't do it.

            // Some other systems (like the post-processing volumes) are
            // also initialized in Awake() and the settings system needs
            // them. It is best to wait until Awake() is done and then init
            // the settings.
        }

        public void Start()
        {
            // We have to call the settings system at least once to initialize the load.
            var _ = Provider.Settings;
        }
    }
}
