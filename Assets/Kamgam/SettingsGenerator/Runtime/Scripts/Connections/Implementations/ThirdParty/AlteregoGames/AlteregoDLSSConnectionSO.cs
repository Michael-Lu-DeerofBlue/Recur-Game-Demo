using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AlteregoDLSSConnection", menuName = "SettingsGenerator/Connection/AlteregoDLSSConnection", order = 4)]
    public class AlteregoDLSSConnectionSO : OptionConnectionSO
    {
        protected AlteregoDLSSConnection _connection;

        /// <summary>
        /// "If enabled then the camera detection will search (an prefer) cameras with the SettingsMainCameraMarker component on it."
        /// </summary>
        [Tooltip("If enabled then the camera detection will  search (an prefer) cameras with the SettingsMainCameraMarker component on it.")]
        public bool CheckForCameraMarker = true;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if (_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AlteregoDLSSConnection();
            _connection.CheckForCameraMarker = CheckForCameraMarker;
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
