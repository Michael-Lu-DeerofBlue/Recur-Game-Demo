using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "RefreshRateConnection", menuName = "SettingsGenerator/Connection/RefreshRateConnection", order = 4)]
    public class RefreshRateConnectionSO : OptionConnectionSO
    {
        /// <summary>
        /// Disable if the refrehs rates change very often.
        /// </summary>
        [Tooltip("Disable if the resolutions change very often.")]
        public bool CacheRefreshRates = true;

        /// <summary>
        /// Refresh rate in Hz. Refresh rates below this are ignored. Equal rates are still included.
        /// </summary>
        [Tooltip("Refresh rate in Hz. Refresh rates below this are ignored. Equal rates are still included.")]
        public int MinRate = 0;

        /// <summary>
        /// Refresh rate in Hz. Refresh rates above this are ignored. Equal rates are still included.
        /// </summary>
        [Tooltip("Refresh rate in Hz. Refresh rates above this are ignored. Equal rates are still included.")]
        public int MaxRate = 1000;

        /// <summary>
        /// If enabled then only those refresh rates which are usable 
        /// with the current resolution are listed. That list may be
        /// much shorter than the full list (often just one).
        /// </summary>
        [Tooltip(
            "If enabled then only those refresh rates which are usable " +
            "with the current resolution are listed. That list may be " +
            "much shorter than the full list (often just one).")]
        public bool LimitToCurrentResolution = false;

        protected RefreshRateConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new RefreshRateConnection();
            _connection.CacheRefreshRates = CacheRefreshRates;
            _connection.MinRate = MinRate;
            _connection.MaxRate = MaxRate;
            _connection.LimitToCurrentResolution = LimitToCurrentResolution;
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
