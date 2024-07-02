using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "InputBindingConnection", menuName = "SettingsGenerator/Connection/InputBindingConnection", order = 4)]
    public class InputBindingConnectionSO : StringConnectionSO
    {
#if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// The input action asset. An override will be added to the binding in this asset if Set(KeyCombination keyCombination) is called.
        /// </summary>
        public InputActionAsset InputActionAsset;

        /// <summary>
        /// A string uniquely identifying the binding.<br />
        /// If you are using the InputSystem then this is the GUID of the Action.<br />
        /// In other systems it may be something else.
        /// </summary>
        public string BindingId;

        protected InputBindingConnection _connection;

        public override IConnection<string> GetConnection()
        {
            if (_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new InputBindingConnection();
            _connection.SetInputActionAsset(InputActionAsset);
            _connection.SetBindingId(BindingId);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
#else

        public override IConnection<string> GetConnection()
        {
            throw new System.NotImplementedException("InputBindingConnectionSO is only available if the InputSystem package is installed. See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5");
        }

        public override void DestroyConnection()
        {
        }
#endif


    }
}
