namespace Kamgam.SettingsGenerator
{
    // The ConnectionSOs are just wrappers for connections so we
    // can used them as ScriptableObjects.

    public interface IConnectionSO<TConnection>
    {
        TConnection GetConnection();
        void DestroyConnection();
    }
}
