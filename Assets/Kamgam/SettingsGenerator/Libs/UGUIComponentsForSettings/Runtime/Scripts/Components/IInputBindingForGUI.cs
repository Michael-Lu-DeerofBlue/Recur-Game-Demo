namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// Interface for input binding GUI. Use this to add your own input binding.
    /// The default implementation assumes the Unity InputSystem is used.
    /// </summary>
    public interface IInputBindingForGUI
    {
        string GetBindingPath();
        void SetBindingPath(string path);

        void StartListening();
        void AddOnCompleteCallback(System.Action callback);
        void RemoveOnCompleteCallback(System.Action callback);
        void AddOnCanceledCallback(System.Action callback);
        void RemoveOnCanceledCallback(System.Action callback);

        void OnEnable();
        void OnDisable();
    }
}
